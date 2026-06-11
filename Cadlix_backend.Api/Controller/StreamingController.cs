using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/media")]
    [ApiController]
    public class StreamingController : ControllerBase
    {
        private static readonly HttpClient _httpClient = new(new HttpClientHandler { AllowAutoRedirect = true });

        private readonly ILogger<StreamingController> _logger;
        private readonly string _videoStoragePath;
        private readonly string _posterPath;
        private readonly string _thumbnailPath;
        private readonly string _backdropPath;
        private readonly string _defaultPath;

        public StreamingController(ILogger<StreamingController> logger)
        {
            _logger = logger;
            _videoStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "videos");
            _posterPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "images", "posters");
            _thumbnailPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "images", "thumbnails");
            _backdropPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "images", "backdrops");
            _defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "images", "defaults");

            EnsureDirectoriesExist();
        }

        private void EnsureDirectoriesExist()
        {
            foreach (var dir in new[] { _videoStoragePath, _posterPath, _thumbnailPath, _backdropPath, _defaultPath })
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// Stream video file with support for range requests
        /// </summary>
        [HttpGet("video/{fileName}")]
        [AllowAnonymous]
        public async Task<IActionResult> StreamVideo(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || fileName.Contains(".."))
                    return BadRequest("Invalid file name");

                var filePath = ResolveFilePath(fileName, _videoStoragePath);
                if (filePath == null || !System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"Video file not found: {fileName}");
                    return NotFound("Video not found");
                }

                return await StreamFileWithRange(filePath, GetVideoContentType(fileName));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error streaming video: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error streaming video");
            }
        }

        /// <summary>
        /// Serve image file (poster, thumbnail, backdrop, default)
        /// </summary>
        [HttpGet("image/{*filePath}")]
        [AllowAnonymous]
        public IActionResult ServeImage(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || filePath.Contains(".."))
                    return BadRequest("Invalid file path");

                var parts = filePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    return BadRequest("Invalid file path");

                string directory;
                string fileName;

                if (parts.Length == 1)
                {
                    directory = _defaultPath;
                    fileName = parts[0];
                }
                else
                {
                    var folder = parts[0].ToLowerInvariant();
                    fileName = parts[1];

                    directory = folder switch
                    {
                        "posters" => _posterPath,
                        "thumbnails" => _thumbnailPath,
                        "backdrops" => _backdropPath,
                        "defaults" => _defaultPath,
                        _ => string.Empty
                    };

                    if (string.IsNullOrEmpty(directory))
                        return BadRequest("Invalid image type");
                }

                var fullPath = Path.Combine(directory, fileName);
                if (!System.IO.File.Exists(fullPath))
                {
                    var defaultImage = Path.Combine(_defaultPath, "default-poster.png");
                    if (System.IO.File.Exists(defaultImage))
                        fullPath = defaultImage;
                    else
                        return NotFound("Image not found");
                }

                var contentType = GetImageContentType(fileName);
                return PhysicalFile(fullPath, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error serving image: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error serving image");
            }
        }

        /// <summary>
        /// Stream video file (legacy route for backward compatibility)
        /// </summary>
        [HttpGet("streaming/{fileName}")]
        [AllowAnonymous]
        public async Task<IActionResult> StreamVideoLegacy(string fileName)
        {
            return await StreamVideo(fileName);
        }

        /// <summary>
        /// Proxy external video URL through the backend to avoid CORS/hotlink restrictions
        /// </summary>
        [HttpGet("video/proxy")]
        [AllowAnonymous]
        public async Task ProxyVideo([FromQuery] string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("https://"))
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("Invalid URL");
                    return;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                var rangeHeader = Request.Headers["Range"].FirstOrDefault();
                if (!string.IsNullOrEmpty(rangeHeader))
                    request.Headers.TryAddWithoutValidation("Range", rangeHeader);

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.PartialContent)
                {
                    Response.StatusCode = (int)response.StatusCode;
                    await Response.WriteAsync("Error proxying video");
                    return;
                }

                Response.StatusCode = (int)response.StatusCode;
                Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "video/mp4";
                Response.Headers["Accept-Ranges"] = "bytes";

                if (response.Content.Headers.ContentLength.HasValue)
                    Response.Headers["Content-Length"] = response.Content.Headers.ContentLength.Value.ToString();

                var contentRange = response.Content.Headers.ContentRange?.ToString();
                if (!string.IsNullOrEmpty(contentRange))
                    Response.Headers["Content-Range"] = contentRange;

                await response.Content.CopyToAsync(Response.Body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error proxying video: {ex.Message}");
                if (!Response.HasStarted)
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("Error proxying video");
                }
            }
        }

        /// <summary>
        /// HEAD request for video file info
        /// </summary>
        [HttpHead("video/{fileName}")]
        [AllowAnonymous]
        public IActionResult GetVideoInfo(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || fileName.Contains(".."))
                    return BadRequest("Invalid file name");

                var filePath = ResolveFilePath(fileName, _videoStoragePath);
                if (filePath == null || !System.IO.File.Exists(filePath))
                    return NotFound();

                var fileInfo = new FileInfo(filePath);
                Response.Headers["Content-Length"] = fileInfo.Length.ToString();
                Response.Headers["Accept-Ranges"] = "bytes";
                Response.ContentType = GetVideoContentType(fileName);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting video info: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private string? ResolveFilePath(string fileName, string basePath)
        {
            var fullPath = Path.Combine(basePath, fileName);
            if (System.IO.File.Exists(fullPath))
                return fullPath;

            var files = Directory.GetFiles(basePath);
            foreach (var file in files)
            {
                if (Path.GetFileName(file) == fileName)
                    return file;
            }

            return null;
        }

        private async Task<IActionResult> StreamFileWithRange(string filePath, string contentType)
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType, enableRangeProcessing: true);
        }

        private string GetVideoContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                ".mkv" => "video/x-matroska",
                ".avi" => "video/x-msvideo",
                ".mov" => "video/quicktime",
                ".flv" => "video/x-flv",
                ".wmv" => "video/x-ms-wmv",
                ".m4v" => "video/x-m4v",
                _ => "video/mp4"
            };
        }

        private string GetImageContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".svg" => "image/svg+xml",
                _ => "image/jpeg"
            };
        }
    }
}

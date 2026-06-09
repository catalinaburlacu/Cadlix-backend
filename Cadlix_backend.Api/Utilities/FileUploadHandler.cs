using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Fonts;

namespace Cadlix_backend.Api.Utilities
{
    public class FileUploadHandler
    {
        private readonly ILogger _logger;
        private readonly string _videoUploadPath;
        private readonly string _imageUploadPath;
        private readonly string _posterPath;
        private readonly string _thumbnailPath;
        private readonly string _backdropPath;
        private readonly string _defaultPath;
        private readonly long _maxVideoSizeBytes;
        private readonly long _maxImageSizeBytes;

        private static readonly string[] AllowedVideoFormats = { ".mp4", ".avi", ".mkv", ".mov", ".webm", ".flv", ".wmv", ".m4v" };
        private static readonly string[] AllowedImageFormats = { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp" };

        public FileUploadHandler(ILogger logger, long maxVideoSizeBytes = 5368709120, long maxImageSizeBytes = 10485760)
        {
            _logger = logger;
            _maxVideoSizeBytes = maxVideoSizeBytes;
            _maxImageSizeBytes = maxImageSizeBytes;

            _videoUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "videos");
            _imageUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "images");
            _posterPath = Path.Combine(_imageUploadPath, "posters");
            _thumbnailPath = Path.Combine(_imageUploadPath, "thumbnails");
            _backdropPath = Path.Combine(_imageUploadPath, "backdrops");
            _defaultPath = Path.Combine(_imageUploadPath, "defaults");

            EnsureDirectoriesExist();
            GenerateDefaultImagesIfMissing();
        }

        private void EnsureDirectoriesExist()
        {
            foreach (var dir in new[] { _videoUploadPath, _imageUploadPath, _posterPath, _thumbnailPath, _backdropPath, _defaultPath })
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    _logger.LogInformation($"Created directory: {dir}");
                }
            }
        }

        private void GenerateDefaultImagesIfMissing()
        {
            GenerateDefaultIfMissing("default-poster.png", 300, 450, "#1a1a2e", "#e94560", "No Poster");
            GenerateDefaultIfMissing("default-thumbnail.png", 300, 170, "#16213e", "#0f3460", "No Thumbnail");
            GenerateDefaultIfMissing("default-backdrop.png", 1280, 720, "#0f3460", "#533483", "No Backdrop");
        }

        private void GenerateDefaultIfMissing(string fileName, int width, int height, string bgColor, string accentColor, string text)
        {
            var filePath = Path.Combine(_defaultPath, fileName);
            if (File.Exists(filePath)) return;

            try
            {
                using var image = new Image<Rgba32>(width, height);

                var bg = ParseColor(bgColor);
                var accent = ParseColor(accentColor);

                image.Mutate(ctx =>
                {
                    ctx.Fill(bg);
                });

                image.SaveAsPng(filePath);
                _logger.LogInformation($"Generated default image: {fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not generate default image {fileName}: {ex.Message}. Creating minimal fallback.");
                using var image = new Image<Rgba32>(width, height);
                image.Mutate(ctx => ctx.Fill(ParseColor(bgColor)));
                image.SaveAsPng(filePath);
            }
        }

        private static Rgba32 ParseColor(string hex)
        {
            hex = hex.TrimStart('#');
            if (hex.Length == 6)
            {
                return new Rgba32(
                    Convert.ToByte(hex.Substring(0, 2), 16),
                    Convert.ToByte(hex.Substring(2, 2), 16),
                    Convert.ToByte(hex.Substring(4, 2), 16),
                    255
                );
            }
            return new Rgba32(26, 26, 46, 255);
        }

        public string GetDefaultPoster() => "defaults/default-poster.png";
        public string GetDefaultThumbnail() => "defaults/default-thumbnail.png";
        public string GetDefaultBackdrop() => "defaults/default-backdrop.png";

        public async Task<(bool Success, string? FileName, string? ErrorMessage)> UploadVideoAsync(IFormFile videoFile, int movieId)
        {
            try
            {
                if (videoFile == null || videoFile.Length == 0)
                    return (false, null, "No file provided");

                if (videoFile.Length > _maxVideoSizeBytes)
                    return (false, null, $"File size exceeds maximum limit of {_maxVideoSizeBytes / (1024 * 1024 * 1024)} GB");

                var fileExtension = Path.GetExtension(videoFile.FileName).ToLower();
                if (!Array.Exists(AllowedVideoFormats, element => element == fileExtension))
                    return (false, null, $"File format not supported. Allowed formats: {string.Join(", ", AllowedVideoFormats)}");

                var fileName = $"{movieId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(_videoUploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                }

                _logger.LogInformation($"Video file uploaded successfully: {fileName}");
                return (true, fileName, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading video file: {ex.Message}");
                return (false, null, "Error uploading file: " + ex.Message);
            }
        }

        public async Task<(bool Success, string? FileName, string? ErrorMessage)> UploadImageAsync(IFormFile imageFile, int contentId, string imageType)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return (false, null, "No image file provided");

                if (imageFile.Length > _maxImageSizeBytes)
                    return (false, null, $"Image size exceeds maximum limit of {_maxImageSizeBytes / (1024 * 1024)} MB");

                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!Array.Exists(AllowedImageFormats, element => element == fileExtension))
                    return (false, null, $"Image format not supported. Allowed formats: {string.Join(", ", AllowedImageFormats)}");

                string targetDir = imageType.ToLower() switch
                {
                    "poster" => _posterPath,
                    "thumbnail" => _thumbnailPath,
                    "backdrop" => _backdropPath,
                    _ => _imageUploadPath
                };

                var fileName = $"{contentId}_{imageType}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(targetDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                _logger.LogInformation($"Image uploaded successfully: {fileName} (type: {imageType})");
                return (true, fileName, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading image: {ex.Message}");
                return (false, null, "Error uploading image: " + ex.Message);
            }
        }

        public bool DeleteVideoFile(string? fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || fileName.Contains(".."))
                    return false;

                var filePath = Path.Combine(_videoUploadPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"Video file deleted: {fileName}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting video file {fileName}: {ex.Message}");
                return false;
            }
        }

        public bool DeleteImageFile(string? fileName, string imageType)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || fileName.Contains(".."))
                    return false;

                string targetDir = imageType.ToLower() switch
                {
                    "poster" => _posterPath,
                    "thumbnail" => _thumbnailPath,
                    "backdrop" => _backdropPath,
                    _ => _imageUploadPath
                };

                var filePath = Path.Combine(targetDir, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"Image file deleted: {fileName} (type: {imageType})");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting image file {fileName}: {ex.Message}");
                return false;
            }
        }

        public FileInfo? GetVideoFileInfo(string? fileName)
        {
            return GetFileInfoFromPath(fileName, _videoUploadPath);
        }

        public FileInfo? GetImageFileInfo(string? fileName, string imageType)
        {
            string targetDir = imageType.ToLower() switch
            {
                "poster" => _posterPath,
                "thumbnail" => _thumbnailPath,
                "backdrop" => _backdropPath,
                _ => _imageUploadPath
            };
            return GetFileInfoFromPath(fileName, targetDir);
        }

        private FileInfo? GetFileInfoFromPath(string? fileName, string basePath)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || fileName.Contains(".."))
                    return null;

                var filePath = Path.Combine(basePath, fileName);
                return File.Exists(filePath) ? new FileInfo(filePath) : null;
            }
            catch
            {
                return null;
            }
        }

        public bool VideoFileExists(string? fileName) => FileExistsInPath(fileName, _videoUploadPath);
        public bool ImageFileExists(string? fileName, string imageType)
        {
            string targetDir = imageType.ToLower() switch
            {
                "poster" => _posterPath,
                "thumbnail" => _thumbnailPath,
                "backdrop" => _backdropPath,
                _ => _imageUploadPath
            };
            return FileExistsInPath(fileName, targetDir);
        }

        private bool FileExistsInPath(string? fileName, string basePath)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || fileName.Contains(".."))
                    return false;
                return File.Exists(Path.Combine(basePath, fileName));
            }
            catch
            {
                return false;
            }
        }

        public string GetVideoUploadPath() => _videoUploadPath;
        public string GetImageUploadPath() => _imageUploadPath;
        public string GetPosterPath() => _posterPath;
        public string GetThumbnailPath() => _thumbnailPath;
        public string GetBackdropPath() => _backdropPath;
        public string GetDefaultPath() => _defaultPath;
    }
}

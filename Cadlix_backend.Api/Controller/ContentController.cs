using Cadlix_backend.BusinessLayer;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.Domain.DTOs.Content;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller;

[Route("api/content")]
[ApiController]
public class ContentController : ControllerBase
{
    private readonly IFrontendAction _frontendService;
    private readonly IContentAction _contentService;
    private readonly FileUploadHandler _fileUploadHandler;
    private readonly ILogger<ContentController> _logger;

    public ContentController(ILogger<ContentController> logger)
    {
        _frontendService = new BusinessLogic().Frontend();
        _contentService = new BusinessLogic().Content();
        _fileUploadHandler = new FileUploadHandler(logger);
        _logger = logger;
    }

    [HttpGet("home")]
    public ActionResult<HomePayloadDto> GetHome()
    {
        return Ok(_frontendService.GetHome());
    }

    [HttpGet("trending")]
    public ActionResult<TrendingPayloadDto> GetTrending([FromQuery] string? period, [FromQuery] string? filter)
    {
        return Ok(_frontendService.GetTrending(period, filter));
    }

    [HttpGet("explore")]
    public ActionResult<ExplorePayloadDto> GetExplore()
    {
        return Ok(_frontendService.GetExplore());
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var content = _contentService.GetAllContent().Select(EnrichMediaUrls);
        return Ok(content);
    }

    [HttpGet("type/{type}")]
    public IActionResult GetByType(string type)
    {
        var content = _contentService.GetContentByType(type).Select(EnrichMediaUrls);
        return Ok(content);
    }

    [HttpGet("series/{seriesName}/episodes")]
    public IActionResult GetSeriesEpisodes(string seriesName)
    {
        var episodes = _contentService.GetSeriesEpisodes(seriesName).Select(EnrichMediaUrls);
        return Ok(episodes);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var content = _contentService.GetContentById(id);
        if (content == null)
            return NotFound();

        var enriched = EnrichMediaUrls(content);
        return Ok(enriched);
    }

    [HttpPost("create")]
    [Authorize]
    public IActionResult Create([FromBody] CreateContentDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = _contentService.CreateContent(dto);
        var enriched = EnrichMediaUrls(created);
        return Ok(enriched);
    }

    [HttpPost("upload")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(6L * 1024 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 6L * 1024 * 1024 * 1024)]
    public async Task<IActionResult> UploadContent(
        [FromForm] ContentUploadFormDto form)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(form.Title))
                return BadRequest("Title is required");

            if (string.IsNullOrWhiteSpace(form.Type))
                return BadRequest("Type is required");

            var createDto = new CreateContentDTO
            {
                Title = form.Title,
                Type = form.Type,
                Genres = form.Genres ?? new(),
                Year = form.Year ?? DateTime.UtcNow.Year,
                Score = form.Score ?? 0,
            };

            var contentId = 0;
            try
            {
                var created = _contentService.CreateContent(createDto);
                contentId = created.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating content record: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ContentUploadResponseDTO { Success = false, Message = "Error creating content record" });
            }

            string? videoFileName = null;
            string? posterFileName = null;
            string? thumbnailFileName = null;
            string? backdropFileName = null;

            if (form.VideoFile != null && form.VideoFile.Length > 0)
            {
                var (success, fileName, error) = await _fileUploadHandler.UploadVideoAsync(form.VideoFile, contentId);
                if (success)
                {
                    videoFileName = fileName;
                }
                else
                {
                    _logger.LogWarning($"Video upload failed for content {contentId}: {error}");
                }
            }

            if (form.PosterFile != null && form.PosterFile.Length > 0)
            {
                var (success, fileName, error) = await _fileUploadHandler.UploadImageAsync(form.PosterFile, contentId, "poster");
                if (success)
                {
                    posterFileName = fileName;
                }
            }

            if (form.ThumbnailFile != null && form.ThumbnailFile.Length > 0)
            {
                var (success, fileName, error) = await _fileUploadHandler.UploadImageAsync(form.ThumbnailFile, contentId, "thumbnail");
                if (success)
                {
                    thumbnailFileName = fileName;
                }
            }

            if (form.BackdropFile != null && form.BackdropFile.Length > 0)
            {
                var (success, fileName, error) = await _fileUploadHandler.UploadImageAsync(form.BackdropFile, contentId, "backdrop");
                if (success)
                {
                    backdropFileName = fileName;
                }
            }

            var updateDto = new UpdateContentDTO
            {
                Title = form.Title,
                Type = form.Type,
                Year = form.Year,
                Score = form.Score,
                Description = form.Description,
                Director = form.Director,
                Cast = form.Cast,
                Country = form.Country,
                Category = form.Category,
                Duration = form.Duration,
                DurationSeconds = form.DurationSeconds,
                ExternalId = form.ExternalId,
                IsPrivate = form.IsPrivate,
                VideoSource = videoFileName,
                Poster = posterFileName,
                Thumbnail = thumbnailFileName,
                Backdrop = backdropFileName,
            };

            var updated = _contentService.UpdateContent(contentId, updateDto);
            if (updated == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ContentUploadResponseDTO { Success = false, Message = "Error updating content with media references" });
            }

            var enriched = EnrichMediaUrls(updated);

            return Ok(new ContentUploadResponseDTO
            {
                ContentId = contentId,
                Title = form.Title,
                VideoFileName = videoFileName,
                PosterFileName = posterFileName,
                ThumbnailFileName = thumbnailFileName,
                BackdropFileName = backdropFileName,
                Success = true,
                Message = "Content uploaded successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading content: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ContentUploadResponseDTO { Success = false, Message = "Error uploading content" });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult Update(int id, [FromBody] UpdateContentDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = _contentService.UpdateContent(id, dto);
        if (updated == null)
            return NotFound();

        var enriched = EnrichMediaUrls(updated);
        return Ok(enriched);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var existing = _contentService.GetContentById(id);
        if (existing == null)
            return NotFound();

        if (!string.IsNullOrEmpty(existing.VideoSource))
            _fileUploadHandler.DeleteVideoFile(existing.VideoSource);

        if (!string.IsNullOrEmpty(existing.Poster))
            _fileUploadHandler.DeleteImageFile(existing.Poster, "poster");

        if (!string.IsNullOrEmpty(existing.Thumbnail))
            _fileUploadHandler.DeleteImageFile(existing.Thumbnail, "thumbnail");

        if (!string.IsNullOrEmpty(existing.Backdrop))
            _fileUploadHandler.DeleteImageFile(existing.Backdrop, "backdrop");

        var ok = _contentService.DeleteContent(id);
        if (!ok)
            return NotFound();

        return Ok(new { message = "Content deleted successfully." });
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { message = "Query parameter is required." });

        var results = _contentService.SearchContent(query);
        var enriched = results.Select(EnrichMediaUrls);
        return Ok(enriched);
    }

    [HttpPost("{id}/upload-media")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadMediaForContent(
        int id,
        [FromForm] UploadContentMediaDto media)
    {
        var existing = _contentService.GetContentById(id);
        if (existing == null)
            return NotFound("Content not found");

        var updateDto = new UpdateContentDTO();
        bool hasUpdates = false;

        if (media.VideoFile != null && media.VideoFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(existing.VideoSource))
                _fileUploadHandler.DeleteVideoFile(existing.VideoSource);

            var (success, fileName, error) = await _fileUploadHandler.UploadVideoAsync(media.VideoFile, id);
            if (success)
            {
                updateDto.VideoSource = fileName;
                hasUpdates = true;
            }
        }

        if (media.PosterFile != null && media.PosterFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(existing.Poster))
                _fileUploadHandler.DeleteImageFile(existing.Poster, "poster");

            var (success, fileName, error) = await _fileUploadHandler.UploadImageAsync(media.PosterFile, id, "poster");
            if (success)
            {
                updateDto.Poster = fileName;
                hasUpdates = true;
            }
        }

        if (media.ThumbnailFile != null && media.ThumbnailFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(existing.Thumbnail))
                _fileUploadHandler.DeleteImageFile(existing.Thumbnail, "thumbnail");

            var (success, fileName, error) = await _fileUploadHandler.UploadImageAsync(media.ThumbnailFile, id, "thumbnail");
            if (success)
            {
                updateDto.Thumbnail = fileName;
                hasUpdates = true;
            }
        }

        if (media.BackdropFile != null && media.BackdropFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(existing.Backdrop))
                _fileUploadHandler.DeleteImageFile(existing.Backdrop, "backdrop");

            var (success, fileName, error) = await _fileUploadHandler.UploadImageAsync(media.BackdropFile, id, "backdrop");
            if (success)
            {
                updateDto.Backdrop = fileName;
                hasUpdates = true;
            }
        }

        if (!hasUpdates)
            return BadRequest(new { message = "No valid files provided" });

        var updated = _contentService.UpdateContent(id, updateDto);
        if (updated == null)
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating content" });

        var enriched = EnrichMediaUrls(updated);
        return Ok(enriched);
    }

    private ContentDTO EnrichMediaUrls(ContentDTO dto)
    {
        dto.Poster = ResolveImageUrl(dto.Poster, "poster", _fileUploadHandler.GetDefaultPoster());
        dto.Thumbnail = ResolveImageUrl(dto.Thumbnail, "thumbnail", _fileUploadHandler.GetDefaultThumbnail());
        dto.Backdrop = ResolveImageUrl(dto.Backdrop, "backdrop", _fileUploadHandler.GetDefaultBackdrop());
        dto.VideoSource = ResolveVideoUrl(dto.VideoSource);

        if (dto.VideoSources != null && dto.VideoSources.Count > 0)
        {
            dto.VideoSources = dto.VideoSources
                .ToDictionary(kv => kv.Key, kv => ResolveVideoUrl(kv.Value));
        }
        else
        {
            var resolved = dto.VideoSource;
            dto.VideoSources = string.IsNullOrEmpty(resolved)
                ? null
                : new Dictionary<string, string>
                {
                    ["Auto"] = resolved,
                    ["1080p"] = resolved,
                    ["720p"] = resolved,
                    ["480p"] = resolved,
                    ["360p"] = resolved,
                };
        }

        return dto;
    }

    private string ResolveImageUrl(string? value, string type, string defaultPath)
    {
        if (string.IsNullOrEmpty(value))
            return $"/api/media/image/{defaultPath}";
        if (value.StartsWith("http://") || value.StartsWith("https://"))
            return value;
        if (!_fileUploadHandler.ImageFileExists(value, type))
            return $"/api/media/image/{defaultPath}";
        return $"/api/media/image/{type}s/{value}";
    }

    private string ResolveVideoUrl(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        if (value.StartsWith("http://") || value.StartsWith("https://"))
            return $"/api/media/video/proxy?url={Uri.EscapeDataString(value)}";
        if (!_fileUploadHandler.VideoFileExists(value))
            return string.Empty;
        return $"/api/media/video/{value}";
    }
}

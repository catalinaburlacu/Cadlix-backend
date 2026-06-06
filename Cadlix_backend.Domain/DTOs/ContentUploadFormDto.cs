using Microsoft.AspNetCore.Http;

namespace Cadlix_backend.Domain.DTOs;

public class ContentUploadFormDto
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
    public int? Year { get; set; }
    public double? Score { get; set; }
    public string? Description { get; set; }
    public string? Director { get; set; }
    public List<string>? Cast { get; set; }
    public List<string>? Country { get; set; }
    public string? Category { get; set; }
    public string? Duration { get; set; }
    public int? DurationSeconds { get; set; }
    public string? ExternalId { get; set; }
    public bool IsPrivate { get; set; }

    public IFormFile? VideoFile { get; set; }
    public IFormFile? PosterFile { get; set; }
    public IFormFile? ThumbnailFile { get; set; }
    public IFormFile? BackdropFile { get; set; }
}

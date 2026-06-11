using System.ComponentModel.DataAnnotations;

namespace Cadlix_backend.Api;

public class UploadMovieFormDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public string? Director { get; set; }

    public List<string>? Genres { get; set; }

    public List<string>? Country { get; set; }

    public List<string>? Cast { get; set; }

    public string? Category { get; set; }

    public string? Duration { get; set; }

    public int? DurationSeconds { get; set; }

    public IFormFile? VideoFile { get; set; }
}

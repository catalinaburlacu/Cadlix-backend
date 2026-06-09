using System.Collections.Generic;

namespace Cadlix_backend.Domain.DTOs.Content;

public class ContentDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public int? Year { get; set; }
    public List<string>? Genres { get; set; }
    public List<string>? Country { get; set; }
    public string? Director { get; set; }
    public List<string>? Cast { get; set; }
    public string? Duration { get; set; }
    public double? Rating { get; set; }
    public double? Score { get; set; }
    public int? Rank { get; set; }
    public string? TrendPercentage { get; set; }
    public string? Views { get; set; }
    public string? Series { get; set; }
    public string? Episode { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Description { get; set; }
    public string? Poster { get; set; }
    public string? Thumbnail { get; set; }
    public string? Backdrop { get; set; }
    public string? VideoSource { get; set; }
    public bool IsPrivate { get; set; }
}

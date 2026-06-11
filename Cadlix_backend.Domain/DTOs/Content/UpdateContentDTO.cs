using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cadlix_backend.Domain.DTOs.Content;

public class UpdateContentDTO
{
    [StringLength(100, MinimumLength = 1)]
    public string? Title { get; set; }

    [StringLength(50)]
    public string? ExternalId { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    public int? Year { get; set; }

    public List<string>? Genres { get; set; }

    public List<string>? Country { get; set; }

    [StringLength(200)]
    public string? Director { get; set; }

    public List<string>? Cast { get; set; }

    [StringLength(50)]
    public string? Duration { get; set; }

    [Range(0, 10)]
    public double? Score { get; set; }

    public int? Rank { get; set; }

    [StringLength(30)]
    public string? TrendPercentage { get; set; }

    [StringLength(30)]
    public string? Views { get; set; }

    [StringLength(100)]
    public string? Series { get; set; }

    [StringLength(100)]
    public string? Episode { get; set; }

    public int? DurationSeconds { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [Url]
    [StringLength(1000)]
    public string? Poster { get; set; }

    [Url]
    [StringLength(1000)]
    public string? Thumbnail { get; set; }

    [Url]
    [StringLength(1000)]
    public string? Backdrop { get; set; }

    [Url]
    [StringLength(1000)]
    public string? VideoSource { get; set; }

    public bool? IsPrivate { get; set; }
}

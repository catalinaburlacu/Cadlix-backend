using System.ComponentModel.DataAnnotations;

namespace Cadlix_backend.Domain.DTOs.History;

public class UpdateHistoryDTO
{
    [StringLength(50)]
    public string? ExternalMovieId { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string? MovieTitle { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(100)]
    public string? Series { get; set; }

    [StringLength(100)]
    public string? Episode { get; set; }

    public DateTime? WatchedAt { get; set; }

    [StringLength(20, MinimumLength = 2)]
    public string? WatchStatus { get; set; }

    [Range(0, 100)]
    public int? ProgressPercentage { get; set; }

    [StringLength(20)]
    public string? Progress { get; set; }

    [Range(0.0, 10.0)]
    public double? UserRating { get; set; }
}
namespace Cadlix_backend.Domain.DTOs.History;

public class HistoryDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public string? ExternalMovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Series { get; set; }
    public string? Episode { get; set; }
    public DateTime WatchedAt { get; set; }
    public string WatchStatus { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public string? Progress { get; set; }
    public double? UserRating { get; set; }
    public string? Poster { get; set; }
}

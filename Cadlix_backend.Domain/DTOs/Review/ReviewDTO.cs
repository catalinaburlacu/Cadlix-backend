namespace Cadlix_backend.Domain.DTOs.Review;

public class ReviewDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public int MovieId { get; set; }
    public double Rating { get; set; }
    public string? Text { get; set; }
    public int LikesCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

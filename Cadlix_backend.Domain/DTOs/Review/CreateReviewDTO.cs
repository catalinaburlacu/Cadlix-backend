using System.ComponentModel.DataAnnotations;

namespace Cadlix_backend.Domain.DTOs.Review;

public class CreateReviewDTO
{
    [Required]
    public int MovieId { get; set; }

    [Range(0, 10)]
    public double Rating { get; set; }

    [StringLength(2000)]
    public string? Text { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cadlix_backend.Domain.Entities.User;
using Cadlix_backend.Domain.Entities.Movie;

namespace Cadlix_backend.Domain.Entities.Review;

public class ReviewData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int MovieId { get; set; }

    [Range(0, 10)]
    public double Rating { get; set; }

    [StringLength(2000)]
    public string? Text { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserData? User { get; set; }

    public int LikesCount { get; set; }

    [ForeignKey(nameof(MovieId))]
    public MovieData? Movie { get; set; }

    public ICollection<ReviewLikeData> ReviewLikes { get; set; } = new List<ReviewLikeData>();
}

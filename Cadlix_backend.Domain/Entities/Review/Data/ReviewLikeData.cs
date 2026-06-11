using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cadlix_backend.Domain.Entities.User;

namespace Cadlix_backend.Domain.Entities.Review;

public class ReviewLikeData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ReviewId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public UserData? User { get; set; }

    [ForeignKey(nameof(ReviewId))]
    public ReviewData? Review { get; set; }
}

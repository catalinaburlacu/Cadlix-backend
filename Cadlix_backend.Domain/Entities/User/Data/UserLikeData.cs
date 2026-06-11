using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadlix_backend.Domain.Entities.User;

public class UserLikeData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int LikerId { get; set; }

    [Required]
    public int LikedUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(LikerId))]
    public UserData? Liker { get; set; }

    [ForeignKey(nameof(LikedUserId))]
    public UserData? LikedUser { get; set; }
}

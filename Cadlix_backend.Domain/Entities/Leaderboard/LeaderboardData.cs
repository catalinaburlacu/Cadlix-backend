using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadlix_backend.Domain.Entities.Leaderboard;

public class LeaderboardData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Username")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Username cannot be longer than 50 characters.")]
    public string? Username { get; set; }

    [Required]
    [Display(Name = "Score")]
    public int Score { get; set; }
}

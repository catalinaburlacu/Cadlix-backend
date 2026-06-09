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

    [StringLength(50)]
    public string? ExternalUserId { get; set; }

    [StringLength(1000)]
    [Url]
    public string? Avatar { get; set; }

    [StringLength(10)]
    public string? Country { get; set; }

    public int PreviousRank { get; set; }

    public int HoursWatched { get; set; }

    public int MoviesWatched { get; set; }

    public int EpisodesWatched { get; set; }

    [Range(0, 10)]
    public double AverageRating { get; set; }

    public int ReviewsWritten { get; set; }

    public int LikesReceived { get; set; }

    [Required]
    [Display(Name = "Score")]
    public int Score { get; set; }
}

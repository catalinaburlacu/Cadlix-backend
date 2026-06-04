using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadlix_backend.Domain.Entities.User;

public class UserData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Username")]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "Username cannot be longer than 30 characters.")]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? ExternalId { get; set; }

    [StringLength(1000)]
    [Url]
    public string? Avatar { get; set; }

    [StringLength(50)]
    public string? Group { get; set; }

    [StringLength(30)]
    public string? Plan { get; set; }

    [StringLength(30)]
    public string? Status { get; set; }

    public DateTime? JoinedAt { get; set; }

    public int TitlesWatched { get; set; }

    public int ReviewCount { get; set; }

    [Range(0, 10)]
    public double? Rating { get; set; }

    public int Comments { get; set; }

    public int LikesGiven { get; set; }

    public int LikesReceived { get; set; }

    public int HoursWatched { get; set; }

    public int AddedToList { get; set; }

    public int DaysOnSite { get; set; }

    [Required]
    [Display(Name = "Password")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Email")]
    [StringLength(30)]
    public string? Email { get; set; }

    public URole Level { get; set; }

    public ICollection<History.HistoryData>? Histories { get; set; }

    public ICollection<ListsOfUserFilms.ListsData>? MovieLists { get; set; }
}

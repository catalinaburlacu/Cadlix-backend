using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cadlix_backend.Domain.Entities.User;
using Cadlix_backend.Domain.Entities.Movie;

namespace Cadlix_backend.Domain.Entities.ListsOfUserFilms;

public class ListsData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "User ID")]
    public int UserId { get; set; }

    [Required]
    [Display(Name = "Film ID")]
    public int FilmId { get; set; }

    [StringLength(50)]
    public string? ExternalFilmId { get; set; }

    [Required]
    [Display(Name = "Film Title")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Film title must be between 2 and 100 characters long.")]
    public string? FilmTitle { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(100)]
    public string? Genre { get; set; }

    [StringLength(100)]
    public string? Episode { get; set; }

    [Url]
    [StringLength(1000)]
    public string? Poster { get; set; }

    [Required]
    [Display(Name = "Added At")]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Display(Name = "Film status")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Film status must be between 2 and 20 characters long.")]
    public string? FilmStatus { get; set; }

    [Required]
    [Display(Name = "Film rating")]
    [Range(0.0, 10.0, ErrorMessage = "Film rating must be between 0.0 and 10.0.")]
    public double FilmRating { get; set; }

    [Display(Name = "Film score")]
    [Range(0.0, 10.0)]
    public double? FilmScore { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserData? User { get; set; }

    [ForeignKey(nameof(FilmId))]
    public MovieData? Film { get; set; }
}

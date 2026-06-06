using System.ComponentModel.DataAnnotations;

namespace Cadlix_backend.Domain.DTOs.Lists;

public class CreateListDTO
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int FilmId { get; set; }

    [StringLength(50)]
    public string? ExternalFilmId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FilmTitle { get; set; } = string.Empty;

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
    [StringLength(20, MinimumLength = 2)]
    public string FilmStatus { get; set; } = "planned";

    [Range(0.0, 10.0)]
    public double FilmRating { get; set; }

    [Range(0.0, 10.0)]
    public double? FilmScore { get; set; }
}

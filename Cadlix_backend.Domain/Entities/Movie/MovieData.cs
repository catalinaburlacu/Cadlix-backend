using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadlix_backend.Domain.Entities.Movie;

public class MovieData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Title")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string? Title { get; set; }

    [Required]
    [Display(Name = "Genre")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Genre cannot be longer than 100 characters.")]
    public string? Genre { get; set; }

    [Required]
    [Display(Name = "Country")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Country cannot be longer than 50 characters.")]
    public string? Country { get; set; }

    [Required]
    [Display(Name = "Release Date")]
    public DateTime ReleaseDate { get; set; }

    [Required]
    [Display(Name = "Rating")]
    [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10.")]
    public double Rating { get; set; }

    [Required]
    [Display(Name = "Description")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Description cannot be longer than 1000 characters.")]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Link")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Link cannot be longer than 200 characters.")]
    public string? Link { get; set; }
}

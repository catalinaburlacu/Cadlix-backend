using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cadlix_backend.Domain.Entities.Categories;

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

    [StringLength(50)]
    public string? ExternalId { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    public int? Year { get; set; }

    public ICollection<CategoryData>? Genres { get; set; }

    [StringLength(100, MinimumLength = 1, ErrorMessage = "Country cannot be longer than 100 characters.")]
    public List<string>? Country { get; set; }

    [StringLength(200)]
    public string? Director { get; set; }

    [StringLength(1000)]
    public List<string>? Cast { get; set; }

    [StringLength(50)]
    public string? Duration { get; set; }

    public int? Score { get; set; }

    public int? Rank { get; set; }

    [StringLength(30)]
    public string? TrendPercentage { get; set; }

    [StringLength(30)]
    public string? Views { get; set; }

    [StringLength(100)]
    public string? Series { get; set; }

    [StringLength(100)]
    public string? Episode { get; set; }

    public int? DurationSeconds { get; set; }

    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Description cannot be longer than 2000 characters.")]
    public string? Description { get; set; }

    [Url]
    [StringLength(1000)]
    public string? Poster { get; set; }

    [Url]
    [StringLength(1000)]
    public string? Thumbnail { get; set; }

    [Url]
    [StringLength(1000)]
    public string? Backdrop { get; set; }

    [Url]
    [StringLength(1000)]
    public string? VideoSource { get; set; }

    public Dictionary<string, string>? VideoSources { get; set; }

    public bool IsPrivate { get; set; } = false;
}

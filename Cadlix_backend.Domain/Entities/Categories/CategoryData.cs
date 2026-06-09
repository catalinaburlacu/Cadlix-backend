using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadlix_backend.Domain.Entities.Categories;

public class CategoryData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Name")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Name cannot be longer than 50 characters.")]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? ExternalId { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    [StringLength(50)]
    public string? Icon { get; set; }

    [StringLength(2000)]
    public string? Items { get; set; }

    public ICollection<Movie.MovieData>? Movies { get; set; }
}

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
    
    public ICollection<Movie.MovieData>? Movies { get; set; }
}

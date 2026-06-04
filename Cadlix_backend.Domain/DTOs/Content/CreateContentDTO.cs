using System.ComponentModel.DataAnnotations;

namespace Cadlix_backend.Domain.DTOs.Frontend;

public class CreateContentDTO
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    public List<string> Genres { get; set; } = new();

    [Range(1888, 2100)]
    public int Year { get; set; }

    [Range(0, 10)]
    public double Score { get; set; }
}
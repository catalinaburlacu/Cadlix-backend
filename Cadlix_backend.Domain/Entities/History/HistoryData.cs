using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadlix_backend.Domain.Entities.History;

public class HistoryData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "User ID")]
    public int UserId { get; set; }

    [Required]
    [Display(Name = "Movie ID")]
    public int MovieId { get; set; }

    [Required]
    [Display(Name = "Movie Title")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Movie title must be between 1 and 100 characters long.")]
    public string? MovieTitle { get; set; }

    [Required]
    [Display(Name = "Watched At")]
    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Display(Name = "Watch Status")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Watch status must be between 2 and 20 characters long.")]
    public string? WatchStatus { get; set; }

    [Display(Name = "Progress Percentage")]
    [Range(0, 100, ErrorMessage = "Progress percentage must be between 0 and 100.")]
    public int ProgressPercentage { get; set; }

    [Display(Name = "User Rating")]
    [Range(0.0, 10.0, ErrorMessage = "User rating must be between 0.0 and 10.0.")]
    public double? UserRating { get; set; }
}

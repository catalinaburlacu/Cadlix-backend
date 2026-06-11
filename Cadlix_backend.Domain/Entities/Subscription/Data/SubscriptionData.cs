using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cadlix_backend.Domain.Entities.User;

namespace Cadlix_backend.Domain.Entities.Subscription;

public class SubscriptionData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "User ID")]
    public int UserId { get; set; }

    [Required]
    [Display(Name = "Subscription Name")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Subscription name must be between 3 and 50 characters long.")]
    public string? Name { get; set; }

    [StringLength(20)]
    public string? PlanId { get; set; }

    [StringLength(100)]
    public string? Subtitle { get; set; }

    [StringLength(20)]
    public string? PriceLabel { get; set; }

    [StringLength(20)]
    public string? PriceValue { get; set; }

    [StringLength(100)]
    public string? Cta { get; set; }

    [Required]
    [Display(Name = "Price")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required]
    [Display(Name = "Duration (Months)")]
    [Range(1, 60, ErrorMessage = "Duration must be between 1 and 60 months.")]
    public int DurationInMonths { get; set; }

    [Required]
    [Display(Name = "Features")]
    public string? Features { get; set; }

    public int MaxDevices { get; set; }

    public bool HasAds { get; set; }

    public bool SupportsOfflineDownload { get; set; }

    [StringLength(20)]
    public string? VideoQuality { get; set; }

    [Required]
    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Display(Name = "Is Canceled")]
    public bool IsCanceled { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserData? User { get; set; }
}

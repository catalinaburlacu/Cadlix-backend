using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cadlix_backend.Domain.Enums;


namespace Cadlix_backend.DataAccess.Entities;

public class Users
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Username")]
    [StringLength(30, MinimumLength = 5 ,ErrorMessage ="Username cannot be longer than 30 characters.")]
    public string? Name { get; set; }

    [Required]
    [Display(Name = "Password")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string? Password { get; set; }

    [Required]
    [Display(Name = "Email")]
    [StringLength(30)]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    public DateTime LastLogin { get; set; }

    [StringLength(30)]
    public string? LasIp { get; set; }
    
    public URole Level { get; set; }
}

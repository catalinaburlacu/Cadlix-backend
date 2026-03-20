using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadlix_backend.Domain.Entities.ListsOfUserFilms;

public class ListsData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "List Name")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "List name must be between 3 and 50 characters long.")]
    public string? Name { get; set; }

    [Required]
    [Display(Name = "User ID")]
    public int UserId { get; set; }
}

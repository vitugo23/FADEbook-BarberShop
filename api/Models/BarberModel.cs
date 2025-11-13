
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Models;

public class BarberModel : AModel
{
    [Key]
    public int BarberId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name must be between 1 and 50 characters.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, ErrorMessage = "Username must be between 1 and 50 characters.")]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(100, ErrorMessage = "Name must be between 1 and 100 characters.")]
    public string Specialty { get; set; } = "";

    [Required]
    [StringLength(50, ErrorMessage = "Name must be between 1 and 50 characters.")]
    public string ContactInfo { get; set; } = "";

    // Navigation property for explicit many-to-many relationship
    public ICollection<BarberServiceModel> BarberServices { get; set; } = new List<BarberServiceModel>();
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Models;

public class ServiceModel: AModel
{
    [Key]
    public int ServiceId { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Service name must be between 1 and 50 characters.")]
    public string ServiceName { get; set; } = "";

    [Required]
    [Range(0, 1000)]
    public double ServicePrice { get; set; }

    // Navigation property for explicit many-to-many relationship
    public ICollection<BarberServiceModel> BarberServices { get; set; } = new List<BarberServiceModel>();
}
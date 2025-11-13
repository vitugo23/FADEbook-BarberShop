using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Models;

public class AppointmentModel : AModel
{
    // KEYS WILL BE PUBLIC IN DTO
    [Key]
    public int AppointmentId { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";
    
    [Required]
    public DateTime AppointmentDate { get; set; } = DateTime.MinValue;

    [Required]
    public int CustomerId { get; set; } // Foreign key
    [ForeignKey(nameof(CustomerId))]
    public CustomerModel Customer { get; set; } = null!;

    [Required]
    public int ServiceId { get; set; } // Foreign key
    [ForeignKey(nameof(ServiceId))]
    public ServiceModel Service { get; set; } = null!;

    [Required]
    public int BarberId { get; set; } // Foreign key
    [ForeignKey(nameof(BarberId))]
    public BarberModel Barber { get; set; } = null!;
}

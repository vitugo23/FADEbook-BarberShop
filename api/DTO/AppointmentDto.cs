using System.ComponentModel.DataAnnotations;

namespace Fadebook.DTOs;

public class AppointmentDto
{
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(50, ErrorMessage = "Status must be between 1 and 50 characters.")]
    public string Status { get; set; } = string.Empty;

    [Required(ErrorMessage = "Customer ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive number.")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Service ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Service ID must be a positive number.")]
    public int ServiceId { get; set; }

    [Required(ErrorMessage = "Barber ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Barber ID must be a positive number.")]
    public int BarberId { get; set; }

    [Required(ErrorMessage = "Appointment date is required.")]
    public DateTime appointmentDate { get; set; }
}

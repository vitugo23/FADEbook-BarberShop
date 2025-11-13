using System.ComponentModel.DataAnnotations;

namespace Fadebook.DTOs;

public class ServiceDto
{
    public int ServiceId { get; set; }

    [Required(ErrorMessage = "Service name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Service name must be between 1 and 100 characters.")]
    public string ServiceName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Service price is required.")]
    [Range(0.01, 10000.00, ErrorMessage = "Service price must be between $0.01 and $10,000.00.")]
    public double ServicePrice { get; set; }
}
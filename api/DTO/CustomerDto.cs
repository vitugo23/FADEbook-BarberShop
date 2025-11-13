using System.ComponentModel.DataAnnotations;

namespace Fadebook.DTOs;

public class CustomerDto
{
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 50 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 50 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact info is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Contact info must be between 1 and 50 characters.")]
    public string ContactInfo { get; set; } = string.Empty;
}
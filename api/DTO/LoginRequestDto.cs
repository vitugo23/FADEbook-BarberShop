using System.ComponentModel.DataAnnotations;

namespace Fadebook.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 50 characters.")]
    public string Username { get; set; } = string.Empty;
}

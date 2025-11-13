
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Models;

public class CustomerModel: AModel
{
    [Key]
    public int CustomerId { get; set; }
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, ErrorMessage = "Username must be between 1 and 50 characters.")]
    public string Username { get; set; } = null!;
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name must be between 1 and 50 characters.")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Contact info is required.")]
    public string ContactInfo { get; set; }
}
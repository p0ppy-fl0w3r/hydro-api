using System.ComponentModel.DataAnnotations;

namespace Hydro.Models;

public class RegisterModel
{
    [Required]
    public string Username { get; set; } = "";
    [Required]
    public string FirstName { get; set; } = "";
    [Required]
    public string LastName { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
    [Required]
    public string Email { get; set; } = "";
    [Required]
    public int Age { get; set; }
}
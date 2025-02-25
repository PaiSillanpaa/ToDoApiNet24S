using System.ComponentModel.DataAnnotations;

namespace PasswordApi.Models;

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}
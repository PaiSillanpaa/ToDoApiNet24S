namespace PasswordApi.Models;

using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}
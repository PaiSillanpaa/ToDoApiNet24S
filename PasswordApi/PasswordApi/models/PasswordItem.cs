namespace PasswordApi.Models;

public class PasswordItem
{
    public long Id { get; set; }
    public required string Username { get; set; } // Käyttäjätunnus
    public required string Password { get; set; } // Salasana
    public DateTime CreatedAt { get; set; } // Luontiaika
    public DateTime? LastLoginAt { get; set; } // Viimeinen kirjautuminen
    public bool IsLocked { get; set; } // Lukitustila
    public string? AuthToken { get; set; } // Kirjautumistoken
    public DateTime? TokenExpires { get; set; } // Tokenin vanhenemisaika
}
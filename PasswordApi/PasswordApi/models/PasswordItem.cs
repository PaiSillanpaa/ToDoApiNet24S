namespace PasswordApi.Models;

public class PasswordItem
{
    public long Name { get; set; }
    public string? Password { get; set; }
    public bool IsComplete { get; set; }
}
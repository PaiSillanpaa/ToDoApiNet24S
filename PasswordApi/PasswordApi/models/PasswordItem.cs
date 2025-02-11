namespace PasswordApi.Models;

public class PasswordItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public bool IsComplete { get; set; }
}
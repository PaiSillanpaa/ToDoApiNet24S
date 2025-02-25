namespace PasswordApi.Models;

public class UserInfoDto
{
    public required string Username { get; set; }
    public bool IsLocked { get; set; }
    public bool IsLoggedIn { get; set; }
}
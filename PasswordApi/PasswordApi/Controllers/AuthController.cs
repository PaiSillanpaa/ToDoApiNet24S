using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordApi.Models;
using PasswordApi.Services;

namespace PasswordApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly PasswordContext _context;
    private readonly PasswordHashService _passwordHashService;

    public AuthController(PasswordContext context, PasswordHashService passwordHashService)
    {
        _context = context;
        _passwordHashService = passwordHashService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<PasswordItem>> Register(RegisterDto dto)
    {
        if (_context.PasswordItems.Any(x => x.Username == dto.Username))
        {
            return BadRequest("Käyttäjätunnus on jo käytössä");
        }

        var passwordItem = new PasswordItem
        {
            Username = dto.Username,
            Password = _passwordHashService.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
            IsLocked = false
        };

        _context.PasswordItems.Add(passwordItem);
        await _context.SaveChangesAsync();

        return Ok(passwordItem);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto dto)
    {
        var user = await _context.PasswordItems
            .FirstOrDefaultAsync(x => x.Username == dto.Username);

        if (user == null || !_passwordHashService.VerifyPassword(dto.Password, user.Password))
        {
            return Unauthorized("Väärä käyttäjätunnus tai salasana");
        }

        if (user.IsLocked)
        {
            return Unauthorized("Käyttäjätili on lukittu");
        }

        user.LastLoginAt = DateTime.UtcNow;
        user.AuthToken = Guid.NewGuid().ToString();
        user.TokenExpires = DateTime.UtcNow.AddHours(1);
        await _context.SaveChangesAsync();

        return Ok(new { token = user.AuthToken });
    }

    [HttpGet("status")]
    public async Task<ActionResult<bool>> CheckLoginStatus([FromHeader(Name = "Authorization")] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var user = await _context.PasswordItems
            .FirstOrDefaultAsync(x => x.AuthToken == token && x.TokenExpires > DateTime.UtcNow);

        return user != null && !user.IsLocked;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetUsers()
    {
        var users = await _context.PasswordItems
            .Select(u => new UserInfoDto
            {
                Username = u.Username,
                IsLocked = u.IsLocked,
                IsLoggedIn = u.AuthToken != null && u.TokenExpires > DateTime.UtcNow
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("lock/{username}")]
    public async Task<ActionResult> LockUser(string username)
    {
        var user = await _context.PasswordItems
            .FirstOrDefaultAsync(x => x.Username == username);

        if (user == null)
        {
            return NotFound("Käyttäjää ei löydy");
        }

        user.IsLocked = true;
        user.AuthToken = null; // Kirjaa ulos jos oli kirjautuneena
        user.TokenExpires = null;
        await _context.SaveChangesAsync();

        return Ok($"Käyttäjä {username} lukittu");
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromHeader(Name = "Authorization")] string token)
    {
        var user = await _context.PasswordItems
            .FirstOrDefaultAsync(x => x.AuthToken == token);

        if (user != null)
        {
            user.AuthToken = null;
            user.TokenExpires = null;
            await _context.SaveChangesAsync();
        }

        return Ok("Uloskirjautuminen onnistui");
    }
}
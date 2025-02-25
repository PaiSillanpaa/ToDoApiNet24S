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
            CreatedAt = DateTime.UtcNow
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

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok("Kirjautuminen onnistui");
    }
}
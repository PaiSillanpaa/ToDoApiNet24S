using Microsoft.EntityFrameworkCore;
using PasswordApi.Models;

namespace PasswordApi.Models;

public class PasswordContext : DbContext
{
    public PasswordContext(DbContextOptions<PasswordContext> options)
        : base(options)
    {
    }

    public DbSet<PasswordItem> PasswordItems { get; set; } = null!;
}
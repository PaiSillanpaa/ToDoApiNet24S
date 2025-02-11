using Microsoft.EntityFrameworkCore;

namespace PasswordApi.Models;

public class PasswordContext : DbContext
{
    public PasswordContext(DbContextOptions<PasswordContext> options)
        : base(options)
    {
    }

    public DbSet<PasswordItem> PasswordItemsItems { get; set; } = null!;
}
using CarWorkshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarWorkshop.Models;

namespace CarWorkshop.Data;

public class AuthDbContext : IdentityDbContext<User>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
    }

public DbSet<CarWorkshop.Models.BoughtPart> BoughtPart { get; set; } = default!;

public DbSet<CarWorkshop.Models.TimeSlot> TimeSlot { get; set; } = default!;

public DbSet<CarWorkshop.Models.Ticket> Ticket { get; set; } = default!;
}

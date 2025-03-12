using Microsoft.EntityFrameworkCore;
using SmartCraft.Core.Tellus.Infrastructure.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Context;

public class CompanyContext : DbContext
{
	public CompanyContext(DbContextOptions<CompanyContext> options) : base(options)
	{

	}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Company>()
        .HasIndex(c => new { c.Id, c.TenantId })
        .IsUnique();
    }

    public DbSet<Company> Companies { get; set; }
}

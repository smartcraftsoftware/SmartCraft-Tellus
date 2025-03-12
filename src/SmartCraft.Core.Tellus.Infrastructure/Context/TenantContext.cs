using Microsoft.EntityFrameworkCore;
using SmartCraft.Core.Tellus.Infrastructure.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Context;

public class TenantContext : DbContext
{
	public TenantContext(DbContextOptions<TenantContext> options) : base(options)
	{

	}

	public DbSet<Company> Tenants { get; set; }
}

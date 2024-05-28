using Microsoft.EntityFrameworkCore;
using SmartCraft.Core.Tellus.Infrastructure.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Context;

public class VehicleContext : DbContext
{
    public VehicleContext()
    {
    }

    public VehicleContext(DbContextOptions<VehicleContext> options) : base(options)
    {
        
    }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<StatusReport> StatusReports { get; set; }
    public DbSet<EsgVehicleReport> EsgVehicleReports { get; set;}
    public DbSet<VehicleEvaluation> VehicleEvaluations { get; set; }
}

using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Client;
public interface IVehicleClient
{
    string VehicleBrand { get; }
    Task<List<Vehicle>> GetVehiclesAsync(Tenant tenant, string? vin);
    Task<EsgVehicleReport> GetEsgReportAsync(string? vin, Tenant tenant, DateTime startTime, DateTime stopTime);
    Task<IntervalStatusReport> GetVehicleStatusAsync(string vin, Tenant tenant, DateTime startTime, DateTime stopTime);
}

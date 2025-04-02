using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Client;
public interface IVehicleClient
{
    string VehicleBrand { get; }
    Task<List<Vehicle>> GetVehiclesAsync(Company tenant, string? vin);
    Task<EsgVehicleReport> GetEsgReportAsync(string? vin, Company tenant, DateTime startTime, DateTime stopTime);
    Task<IntervalStatusReport> GetVehicleStatusAsync(string vin, Company tenant, DateTime startTime, DateTime stopTime);
}

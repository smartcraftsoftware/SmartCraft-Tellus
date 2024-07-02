using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Client;
public interface IVehicleClient
{
    string VehicleBrand { get; }
    Task<List<Vehicle>> GetVehiclesAsync(Tenant tenant);
    Task<StatusReport> GetVehicleStatusAsync(string vin, Tenant tenant, string startTime, string? stopTime);
    Task<EsgVehicleReport> GetEsgReportAsync(string? vin, Tenant tenant, string startTime, string stopTime);
    Task<IntervalStatusReport> GetIntervalStatusReportAsync(string vin, Tenant tenant, string startTime, string stopTime);
}

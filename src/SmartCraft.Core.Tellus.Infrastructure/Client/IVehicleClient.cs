using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Client;
public interface IVehicleClient
{
    string VehicleBrand { get; }
    Task<List<Domain.Models.Vehicle>> GetVehiclesAsync(Tenant tenant);
    Task<Domain.Models.StatusReport> GetVehicleStatusAsync(string vin, Tenant tenant, DateTime startTime, DateTime? stopTime);
    Task<Domain.Models.EsgVehicleReport> GetEsgReportAsync(string? vin, Tenant tenant, DateTime startTime, DateTime stopTime);
}

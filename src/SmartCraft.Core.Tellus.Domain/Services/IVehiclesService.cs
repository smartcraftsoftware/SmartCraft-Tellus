using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Domain.Services;

public interface IVehiclesService
{
    Task<List<Vehicle>> GetFleetAsync(string vehicleBrand, Tenant tenant);
    Task<StatusReport> GetVehicleStatusAsync(string vehicleBrand, string vin, Tenant tenant, string startTime, string stopTime);

    Task<IntervalStatusReport> GetIntervalVehicleStatusAsync(string vehicleBrand, string vin, Tenant tenant, string startTime, string stopTime);
}

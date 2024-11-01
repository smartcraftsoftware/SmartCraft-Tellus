using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Domain.Services;

public interface IVehiclesService
{
    Task<List<Vehicle>> GetVehiclesAsync(string vehicleBrand, string? VIN, Tenant tenant);

    Task<IntervalStatusReport> GetVehicleStatusAsync(string vehicleBrand, string vin, Tenant tenant, DateTime startTime, DateTime stopTime);
}

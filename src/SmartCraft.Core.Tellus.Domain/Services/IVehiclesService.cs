using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Domain.Services;

public interface IVehiclesService
{
    Task<List<Vehicle>> GetVehiclesAsync(string vehicleBrand, string? VIN, Company tenant);

    Task<IntervalStatusReport> GetVehicleStatusAsync(string vehicleBrand, string vin, Company tenant, DateTime startTime, DateTime stopTime);
}

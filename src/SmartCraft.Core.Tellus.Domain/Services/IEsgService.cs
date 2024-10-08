using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Domain.Services;
public interface IEsgService
{
    Task<EsgVehicleReport> GetEsgReportAsync(string vehicleBrand, string? vin, Tenant tenant, DateTime startTime, DateTime? stopTime);
}

using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Domain.Services;
public interface IVehicleEvaluationService
{
    Task<VehicleEvaluationReport> GetVehicleEvaluationReportAsync(string vehicleBrand, string? vin, Tenant tenant, DateTime startTime, DateTime stopTime);
}

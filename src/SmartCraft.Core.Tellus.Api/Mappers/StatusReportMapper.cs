using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Api.Mappers;

public static class StatusReportMapper
{
    public static IntervalStatusReportResponse ToIntervalRespone(this IntervalStatusReport statusReport)
    {
        return new IntervalStatusReportResponse
        {
            Vin = statusReport.Vin,
            StartDateTime = statusReport.StartDateTime,
            EndDateTime = statusReport.EndDateTime,
            HrTotalVehicleDistance = statusReport.HrTotalVehicleDistance,
            TotalEngineHours = statusReport.TotalEngineHours,
            TotalElectricMotorHours = statusReport.TotalElectricMotorHours,
            EngineTotalFuelUsed = statusReport.EngineTotalFuelUsed,
            TotalGaseousFuelUsed = statusReport.TotalGaseousFuelUsed,
            TotalElectricEnergyUsed = statusReport.TotalElectricEnergyUsed,
        };
    }
}
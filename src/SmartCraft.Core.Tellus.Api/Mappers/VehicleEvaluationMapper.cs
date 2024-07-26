using SmartCraft.Core.Tellus.Api.Contracts.Responses;
using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Api.Mappers;

public static class VehicleEvaluationMapper
{
    public static VehicleEvaluationReportResponse ToResponse(this VehicleEvaluationReport report)
    {
        return new VehicleEvaluationReportResponse
        {
            StartTime = report.StartTime,
            StopTime = report.StopTime,
            VehicleEvaluations = report.VehicleEvaluations.Select(x => x.ToResponse()).ToList()
        };
    }

    private static VehicleEvaluationResponse ToResponse(this VehicleEvaluation vehicleEvaluation)
    {
        return new VehicleEvaluationResponse
        {
            Vin = vehicleEvaluation.Vin,
            TotalEngineTime = vehicleEvaluation.TotalEngineTime,
            AvgSpeed = vehicleEvaluation.AvgSpeed,
            AvgFuelConsumption = vehicleEvaluation.AvgFuelConsumption,
            AvgElectricEnergyConsumption = vehicleEvaluation.AvgElectricEnergyConsumption,
            TotalFuelConsumption = vehicleEvaluation.TotalFuelConsumption,
            FuelConsumptionPerHour = vehicleEvaluation.FuelConsumptionPerHour,
            Co2Emissions = vehicleEvaluation.Co2Emissions,
            Co2Saved = vehicleEvaluation.Co2Saved,
            TotalDistance = vehicleEvaluation.TotalDistance,
            TotalGasUsed = vehicleEvaluation.TotalGasUsed
        };
    }
}

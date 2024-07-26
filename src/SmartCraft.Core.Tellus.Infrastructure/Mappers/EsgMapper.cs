using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;

namespace SmartCraft.Core.Tellus.Infrastructure.Mappers;
public static class EsgMapper
{
    #region Infra/Domain
    public static Domain.Models.VehicleEvaluationReport ToDomainModel(this Infrastructure.Models.VehicleEvaluationReport vehicleReport)
    {
        return new Domain.Models.VehicleEvaluationReport
        {
            StartTime = vehicleReport.StartTime,
            StopTime = vehicleReport.StopTime,
            VehicleEvaluations = vehicleReport.VehicleEvaluations.Select(x => x.ToDomainModel()).ToList()
        };
    }
    public static Domain.Models.VehicleEvaluation ToDomainModel(this Infrastructure.Models.VehicleEvaluation evaluation)
    {
        return new Domain.Models.VehicleEvaluation
        {
            AvgElectricEnergyConsumption = evaluation.AvgElectricEnergyConsumption,
            AvgFuelConsumption = evaluation.AvgFuelConsumption,
            AvgSpeed = evaluation.AvgSpeed,
            Co2Emissions = evaluation.Co2Emissions,
            Co2Saved = evaluation.Co2Saved,
            FuelConsumptionPerHour = evaluation.FuelConsumptionPerHour,
            TotalDistance = evaluation.TotalDistance,
            TotalEngineTime = evaluation.TotalEngineTime,
            TotalFuelConsumption = evaluation.TotalFuelConsumption,
            TotalGasUsed = evaluation.TotalGasUsed,
            Vin = evaluation.Vin
        };
    }
    public static Infrastructure.Models.VehicleEvaluationReport ToDataModel(this Domain.Models.VehicleEvaluationReport vehicleReport)
    {
        return new Infrastructure.Models.VehicleEvaluationReport
        {
            StartTime = vehicleReport.StartTime.ToUniversalTime(),
            StopTime = vehicleReport.StopTime?.ToUniversalTime(),
            VehicleEvaluations = vehicleReport.VehicleEvaluations.Select(x => x.ToDataModel()).ToList()
        };
    }
    private static Infrastructure.Models.VehicleEvaluation ToDataModel(this Domain.Models.VehicleEvaluation vehicleEvaluation)
    {
        return new Infrastructure.Models.VehicleEvaluation
        {
            Id = vehicleEvaluation.Id,
            Vin = vehicleEvaluation.Vin,
            AvgElectricEnergyConsumption = vehicleEvaluation.AvgElectricEnergyConsumption,
            AvgFuelConsumption = vehicleEvaluation.AvgFuelConsumption,
            AvgSpeed = vehicleEvaluation.AvgSpeed,
            Co2Emissions = vehicleEvaluation.Co2Emissions,
            Co2Saved = vehicleEvaluation.Co2Saved,
            FuelConsumptionPerHour = vehicleEvaluation.FuelConsumptionPerHour,
            TotalDistance = vehicleEvaluation.TotalDistance,
            TotalEngineTime = vehicleEvaluation.TotalEngineTime,
            TotalFuelConsumption = vehicleEvaluation.TotalFuelConsumption,
            TotalGasUsed = vehicleEvaluation.TotalGasUsed,
        };
    }
    #endregion

    #region Man
    public static Domain.Models.VehicleEvaluationReport ToDomainModel(this ManPerformApiResponse apiResponse)
    {
        DateTime startTime;
        var startTimeParse = DateTime.TryParse(apiResponse?.StartTime, out startTime);
        DateTime stopTime;
        var stopTimeParse = DateTime.TryParse(apiResponse?.StopTime, out stopTime);
        if (!startTimeParse || !stopTimeParse)
            throw new FormatException("Invalid date format");

        TimeSpan timeDifference = stopTime - startTime;
        double totalHours = timeDifference.TotalHours;

        return new Domain.Models.VehicleEvaluationReport
        {
            StartTime = startTime,
            StopTime = stopTime,
            VehicleEvaluations = apiResponse?.Items?.Select(x => x.ToDomainModel(totalHours)).ToList() ?? new List<Domain.Models.VehicleEvaluation>()
        };
    }
    private static Domain.Models.VehicleEvaluation ToDomainModel(this ManPerformItem apiResponse, double totalHours)
    {

        return new Domain.Models.VehicleEvaluation
        {
            AvgSpeed = apiResponse?.AvgSpeed,
            TotalDistance = apiResponse?.Mileage,
            TotalFuelConsumption = apiResponse?.FuelConsumption,
            Co2Emissions = apiResponse?.Co2Emissions,
            FuelConsumptionPerHour = apiResponse?.FuelConsumption / totalHours,
            
        };
    }
    #endregion

    #region Volvo
    public static Domain.Models.VehicleEvaluationReport ToDomainModel(this VolvoUtilizationScoreApiResponse apiResponse)
    {
        DateTime startTime;
        var startTimeParse = DateTime.TryParse(apiResponse?.VuScoreResponse?.StartTime, out startTime);
        DateTime stopTime;
        var stopTimeParse = DateTime.TryParse(apiResponse?.VuScoreResponse?.StopTime, out stopTime);
        if(!startTimeParse || !stopTimeParse)
            throw new FormatException("Invalid date format");

        TimeSpan timeDifference = stopTime - startTime;
        double totalHours = timeDifference.TotalHours;

        if (apiResponse?.VuScoreResponse?.Vehicles == null)
            return new Domain.Models.VehicleEvaluationReport
            {
                StartTime = startTime,
                StopTime = stopTime,
                VehicleEvaluations = apiResponse?.VuScoreResponse?.Fleet?.ToDomainModel(totalHours) ?? new List<Domain.Models.VehicleEvaluation>()
            };

        return new Domain.Models.VehicleEvaluationReport
        {
            StartTime = startTime,
            StopTime = stopTime,
            VehicleEvaluations = apiResponse?.VuScoreResponse?.Vehicles.Select(x => x.ToDomainModel(totalHours)).ToList() ?? new List<Domain.Models.VehicleEvaluation>()
        };
    }
    private static List<Domain.Models.VehicleEvaluation> ToDomainModel(this FleetResponse fleet, double totalHours)
    {
        double? totalFuelConsumption = fleet?.AvgFuelConsumption * totalHours;

        return new List<Domain.Models.VehicleEvaluation>
        {
            new Domain.Models.VehicleEvaluation
            {
                AvgSpeed = fleet?.AvgSpeed,
                AvgFuelConsumption = fleet?.AvgFuelConsumption,
                TotalDistance = fleet?.TotalDistance,
                TotalFuelConsumption = totalFuelConsumption,
                FuelConsumptionPerHour = totalFuelConsumption / totalHours,
            }
        };
    }
    private static Domain.Models.VehicleEvaluation ToDomainModel(this UtilizationVehicle vehicle, double totalHours)
    {

        double? totalFuelConsumption = totalHours * vehicle?.AvgFuelConsumption;

        return new Domain.Models.VehicleEvaluation
        {
            AvgSpeed = vehicle?.AvgSpeed,
            AvgFuelConsumption = vehicle?.AvgFuelConsumption,
            TotalDistance = vehicle?.TotalDistance,
            TotalFuelConsumption = totalFuelConsumption,
            FuelConsumptionPerHour = totalFuelConsumption / totalHours,
        };
    }
    #endregion

    #region Scania
    public static Domain.Models.VehicleEvaluationReport ToDomainModel(this ScaniaVehicleEvaluationApiResponse apiResponse, DateTime startTime, DateTime? stopTime)
    {
        return new Domain.Models.VehicleEvaluationReport
        {
            StartTime = startTime,
            StopTime = stopTime,
            VehicleEvaluations = apiResponse?.VehicleList?.Select(x => x.ToDomainModel()).ToList() ?? new List<Domain.Models.VehicleEvaluation>()
        };
    }

    private static Domain.Models.VehicleEvaluation ToDomainModel(this EvaluationVehicle apiResponse)
    {
        double avgFuelConsumption;
        double.TryParse(apiResponse?.AvgFuelConsumption, System.Globalization.CultureInfo.InvariantCulture, out avgFuelConsumption);
        double avgSpeed;
        double.TryParse(apiResponse?.AvgSpeed, System.Globalization.CultureInfo.InvariantCulture, out avgSpeed);
        double totalDistance;
        double.TryParse(apiResponse?.Distance, System.Globalization.CultureInfo.InvariantCulture, out totalDistance);
        double fuelConsumptionPerHour;
        double.TryParse(apiResponse?.FuelConsumptionPerHour, System.Globalization.CultureInfo.InvariantCulture, out fuelConsumptionPerHour);
        double totalFuelConsumption;
        double.TryParse(apiResponse?.TotalFuelConsumption, System.Globalization.CultureInfo.InvariantCulture, out totalFuelConsumption);
       
        return new Domain.Models.VehicleEvaluation
        {
            AvgSpeed = avgSpeed,
            AvgFuelConsumption = avgFuelConsumption,
            TotalDistance = totalDistance,
            TotalFuelConsumption = totalFuelConsumption,
            FuelConsumptionPerHour = fuelConsumptionPerHour,
            EngineRunningTime = apiResponse?.EngineRunningTime?.ToString()
        };
    }

    #endregion


}

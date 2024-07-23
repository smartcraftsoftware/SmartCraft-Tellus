namespace SmartCraft.Core.Tellus.Api.Contracts.Responses;

public class VehicleEvaluationReportResponse
{
    public DateTime StartTime { get; set; }
    public DateTime? StopTime { get; set; }
    public required List<VehicleEvaluationResponse>? VehicleEvaluations { get; set; }
}

public class VehicleEvaluationResponse
{
    /// <summary>
    /// Vin number of the vehicle. 17 characters long
    /// </summary>
    public string? Vin { get; set; }
    /// <summary>
    /// Volvo: in seconds
    /// </summary>
    public double? TotalEngineTime { get; set; }
    /// <summary>
    /// Scania: Expressed as HH:MM:SS
    /// </summary>
    public string? EngineRunningTime { get; set; }
    /// <summary>
    /// Average driving speed in km/h
    /// </summary>
    public double? AvgSpeed { get; set; }
    /// <summary>
    /// Volvo: ml/100km
    /// Scania: l/100km
    /// </summary>
    public double? AvgFuelConsumption { get; set; }
    /// <summary>
    /// Wh/100km
    /// </summary>
    public double? AvgElectricEnergyConsumption { get; set; }
    /// <summary>
    /// Volvo: in ml
    /// Scania: in l
    /// </summary>
    public double? TotalFuelConsumption { get; set; }
    /// <summary>
    /// Volvo: in ml/h
    /// Scania: in l/h
    /// </summary>
    public double? FuelConsumptionPerHour { get; set; }
    /// <summary>
    /// Co2 emissions in tons
    /// </summary>
    public double? Co2Emissions { get; set; }
    public double? Co2Saved { get; set; }

    public double? TotalDistance { get; set; }
    public double? TotalGasUsed { get; set; }
}

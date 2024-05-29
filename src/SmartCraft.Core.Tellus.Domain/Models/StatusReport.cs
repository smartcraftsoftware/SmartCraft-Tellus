namespace SmartCraft.Core.Tellus.Domain.Models;

public class StatusReport
{
    public Guid Id { get; set; }
    public string? ExternalId { get; set; }
    public string? Vin { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? ReceivedDateTime { get; set; }
    public double? HrTotalVehicleDistance { get; set; }
    public double? TotalEngineHours { get; set; }
    public double? TotalElectricMotorHours { get; set; }
    public double? EngineTotalFuelUsed { get; set; }
    public double? TotalGaseousFuelUsed { get; set; }
    public double? TotalElectricEnergyUsed { get; set; }
    public AccumulatedData? AccumulatedData { get; set; }
    public SnapShotData? SnapShotData { get; set; }
}

public class AccumulatedData
{
    public double? FuelConsumptionDuringCruiseActive { get; set; }
    public double? FuelConsumptionDuringCruiseActiveGaseous { get; set; }
}

public class SnapShotData
{
    public string? Ignition { get; set; }
    public double? EngineSpeed { get; set; }
    public double? ElectricMotorSpeed { get; set; }
    public string? FuelType { get; set; }
    public double? FuelLevel1 { get; set; }
    public double? FuelLevel2 { get; set; }
    public double? CatalystFuelLevel { get; set; }
    public DistanceToEmpty? EstimatedDistanceToEmpty { get; set; }
}

public class DistanceToEmpty
{
    public double? Total { get; set; }
    public double? Fuel { get; set; }
    public double? Gas { get; set; }
    public double? BatteryPack { get; set; }
}

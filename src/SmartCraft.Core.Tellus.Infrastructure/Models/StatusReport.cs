namespace SmartCraft.Core.Tellus.Infrastructure.Models;

public class StatusReport : BaseDbModel
{
    public Guid Id { get; set; }
    public string? Vin { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? StopTime { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? ReceivedDateTime { get; set; }
    public double? HrTotalVehicleDistance { get; set; }
    public double? TotalEngineHours { get; set; }
    public double? TotalElectricMotorHours { get; set; }
    public double? EngineTotalFuelUsed { get; set; }
    public double? TotalGaseousFuelUsed { get; set; }
    public double? TotalElectricEnergyUsed { get; set; }
    public double? FuelConsumptionDuringCruiseActive { get; set; }
    public double? FuelConsumptionDuringCruiseActiveGaseous { get; set; }
    public string? Ignition { get; set; }
    public double? EngineSpeed { get; set; }
    public double? ElectricMotorSpeed { get; set; }
    public string? FuelType { get; set; }
    public double? FuelLevel1 { get; set; }
    public double? FuelLevel2 { get; set; }
    public double? CatalystFuelLevel { get; set; }
    public double? TotalToEmpty { get; set; }
    public double? FuelToEmpty { get; set; }
    public double? GasToEmpty { get; set; }
    public double? BatteryPack { get; set; }

}

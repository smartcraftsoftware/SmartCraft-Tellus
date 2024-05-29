namespace SmartCraft.Core.Tellus.Api.Contracts.Responses;

public class StatusReportResponse
{
    public string? Vin { get; set; }
    public double? TotalEngineHours { get; set; }
    public double? TotalElectricMotorHours { get; set; }
    public double? HrTotalVehicleDistance { get; set; }
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
    public double? TotalDistanceToEmpty { get; set; }
    public double? FuelDistanceToEmpty { get; set; }
    public double? GasDistanceToEmpty { get; set; }
    public double? BatteryPackDistanceToEmpty { get; set; }


}

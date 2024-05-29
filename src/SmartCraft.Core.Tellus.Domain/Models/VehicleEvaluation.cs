namespace SmartCraft.Core.Tellus.Domain.Models;
public class VehicleEvaluation
{
    public Guid Id { get; set; }
    public string? Vin { get; set; }
    public double? TotalEngineTime { get; set; }
    public double? AvgSpeed { get; set; }
    public double? AvgFuelConsumption { get; set; }
    public double? AvgElectricEnergyConsumption { get; set; }
    public double? TotalFuelConsumption { get; set; }
    public double? FuelConsumptionPerHour { get; set; }
    public double? Co2Emissions { get; set; }
    public double? Co2Saved { get; set; }
    public double? TotalDistance { get; set; }
    public double? TotalGasUsed { get; set; }
    public string? EngineRunningTime { get; set; }
}

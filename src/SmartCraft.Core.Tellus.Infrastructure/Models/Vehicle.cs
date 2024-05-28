namespace SmartCraft.Core.Tellus.Infrastructure.Models;

public class Vehicle : BaseDbModel
{
    public Guid Id { get; set; }
    public string? Vin { get; set; }
    public string? CustomerVehicleName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Brand { get; set; }
    public List<string>? PossibleFuelTypes { get; set; }
    public string? EmissionLevel { get; set; }
    public double? TotalFuelTankVolume { get; set; }
    public double? TotalFuelTankCapacityGaseous { get; set; }
    public double? TotalBatteryPackCapacity { get; set; }
    public List<StatusReport> StatusReports { get; set; } = new();
}

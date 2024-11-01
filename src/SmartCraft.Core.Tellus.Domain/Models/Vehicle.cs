namespace SmartCraft.Core.Tellus.Domain.Models;

public class Vehicle
{
    public Guid Id { get; set; }
    public string? ExternalId { get; set; }
    public string? Vin { get; set; }
    public string? CustomerVehicleName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Brand { get; set; }
    public List<string>? PossibleFuelTypes { get; set; }
    public string? EngineType { get; set; }
    public string? EmissionLevel { get; set; }
    public double? TotalFuelTankVolume { get; set; }
    public double? TotalFuelTankCapacityGaseous { get; set; }
    public double? TotalBatteryPackCapacity { get; set; }
    public int? NoOfAxles { get; set; }
    public string? GearBoxType { get; set; }
    public string? TachographType { get; set; }
    public string? Type { get; set; }
    public VehicleProductionDate? ProductionDate { get; set; }
}

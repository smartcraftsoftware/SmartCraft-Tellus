namespace SmartCraft.Core.Tellus.Api.Contracts.Responses;


public class GetVehicleResponse
{
    public string? Vin { get; set; }
    public string? ExternalId { get; set; }
    public string? CustomerVehicleName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Brand { get; set; }
    public ProductionDate? ProductionDate { get; set; }
    public List<string>? PossibleFuelTypes { get; set; } 
    public string? EmissionLevel { get; set; }
    public double? TotalFuelTankVolume { get; set; }
    public double? TotalFuelTankCapacityGaseous { get; set; }
    public double? TotalBatteryPackCapacity { get; set; }
    public bool MoreDataAvailable { get; set; }

}


public class ProductionDate
{
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
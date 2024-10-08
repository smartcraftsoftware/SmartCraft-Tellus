using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Api.Contracts.Responses;


public class GetVehicleResponse
{
    public string? Vin { get; set; }
    public string? ExternalId { get; set; }
    public string? CustomerVehicleName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Brand { get; set; }
    public ProductionDateResponse? ProductionDate { get; set; }
    public List<string>? PossibleFuelTypes { get; set; } 
    public string? EmissionLevel { get; set; }
    public double? TotalFuelTankVolume { get; set; }
    public int? NoOfAxles { get; set; }
    public string? GearBoxType { get; set; }
    public bool MoreDataAvailable { get; set; }

}
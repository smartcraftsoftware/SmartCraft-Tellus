using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class VolvoVehiclesApiResponse
{
    [JsonPropertyName("Vehicle")]
    public VolvoVehicleResponse[]? Vehicle { get; set; }
}

public class VolvoVehicleResponse
{
    public required string Vin { get; set; }
    public string? CustomerVehicleName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Brand { get; set; }
    public List<string>? PossibleFuelTypes { get; set; }
    public string? EmissionLevel { get; set; }
    public double? TotalFuelTankVolume { get; set; }
    public double? TotalFuelTankCapacityGaseous { get; set; }
    public double? TotalBatteryPackCapacity { get; set; }
    public bool MoreDataAvailable { get; set; }
}

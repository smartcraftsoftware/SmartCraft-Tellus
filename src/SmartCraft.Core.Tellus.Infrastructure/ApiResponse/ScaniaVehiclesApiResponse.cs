using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class ScaniaVehiclesApiResponse
{
    [JsonPropertyName("vehicleResponse")]
    public VehicleResponse? VehicleResponse { get; set; }
}

public class VehicleResponse
{
    [JsonPropertyName("vehicles")]
    public ScaniaVehicle[]? Vehicles { get; set; }
}

public class ScaniaVehicle
{
    [JsonPropertyName("vin")]
    public required string Vin { get; set; }
    [JsonPropertyName("customerVehicleName")]
    public string? CustomerVehicleName { get; set; }
    [JsonPropertyName("registrationNumber")]
    public string? RegistrationNumber { get; set; }
    [JsonPropertyName("brand")]
    public string? Brand { get; set; }
    [JsonPropertyName("possibleFuelType")]
    public List<string>? PossibleFuelTypes { get; set; }
    [JsonPropertyName("emissionLevel")]
    public string? EmissionLevel { get; set; }
    [JsonPropertyName("totalFuelTankVolume")]
    public double? TotalFuelTankVolume { get; set; }
    [JsonPropertyName("totalFuelTankCapacityGaseous")]
    public double? TotalFuelTankCapacityGaseous { get; set; }
    [JsonPropertyName("totalBatteryPackCapacity")]
    public double? TotalBatteryPackCapacity { get; set; }
}





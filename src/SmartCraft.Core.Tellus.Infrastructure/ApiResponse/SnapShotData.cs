using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class SnapShotData
{
    [JsonPropertyName("ignition")]
    public string? Ignition { get; set; }
    [JsonPropertyName("engineSpeed")]
    public double? EngineSpeed { get; set; }
    [JsonPropertyName("electricMotorSpeed")]
    public double? ElectricMotorSpeed { get; set; }
    [JsonPropertyName("fuelType")]
    public string? FuelType { get; set; }
    [JsonPropertyName("fuelLevel1")]
    public double? FuelLevel1 { get; set; }
    [JsonPropertyName("fuelLevel2")]
    public double? FuelLevel2 { get; set; }
    [JsonPropertyName("catalystFuelLevel")]
    public double? CatalystFuelLevel { get; set; }
    [JsonPropertyName("estimatedDistanceToEmpty")]
    public DistanceToEmpty? EstimatedDistanceToEmpty { get; set; }
}

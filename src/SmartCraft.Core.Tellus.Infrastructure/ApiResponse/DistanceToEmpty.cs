using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class DistanceToEmpty
{
    [JsonPropertyName("total")]
    public double? Total { get; set; }
    [JsonPropertyName("fuel")]
    public double? Fuel { get; set; }
    [JsonPropertyName("gas")]
    public double? Gas { get; set; }
    [JsonPropertyName("batteryPack")]
    public double? BatteryPack { get; set; }
}

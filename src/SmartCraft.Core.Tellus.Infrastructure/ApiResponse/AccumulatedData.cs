using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class AccumulatedData
{
    [JsonPropertyName("fuelConsumptionDuringCruiseActive")]
    public double? FuelConsumptionDuringCruiseActive { get; set; }
    [JsonPropertyName("fuelConsumptionDuringCruiseActiveGaseous")]
    public double? FuelConsumptionDuringCruiseActiveGaseous { get; set; }
}

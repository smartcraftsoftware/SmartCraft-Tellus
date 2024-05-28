using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class ScaniaVehicleEvaluationApiResponse
{
    public EvaluationVehicle?[]? VehicleList { get; set; }
    public string? EvaluationStart { get; set; }
    public string? EvaluationEnd { get; set; }
}

public class EvaluationVehicle
{
    public string? Vin { get; set; }
    [JsonPropertyName("AverageFuelConsumption")]
    public string? AvgFuelConsumption { get; set; }
    public string? FuelConsumptionPerHour { get; set; }
    public string? TotalFuelConsumption { get; set; }
    [JsonPropertyName("AverageSpeed")]
    public string? AvgSpeed { get; set; }
    public string? Distance { get; set; }
    public string? EngineRunningTime { get; set; }

}

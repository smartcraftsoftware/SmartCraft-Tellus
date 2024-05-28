using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class VolvoUtilizationScoreApiResponse
{
    [JsonPropertyName("vuScoreResponse")]
    public VolvoUtilizationScore? VuScoreResponse { get; set; }

}

public class VolvoUtilizationScore
{
    [JsonPropertyName("fleet")]
    public FleetResponse? Fleet { get; set; }
    [JsonPropertyName("vehicle")]
    public UtilizationVehicle?[]? Vehicles { get; set; }
    [JsonPropertyName("startTime")]
    public string? StartTime { get; set; }
    [JsonPropertyName("stopTime")]
    public string? StopTime { get; set; }

}

public class FleetResponse
{
    [JsonPropertyName("avgSpeedDriving")]
    public double? AvgSpeed { get; set; }
    //Total time the engine has been running in seconds
    [JsonPropertyName("totalTime")]
    public int TotalEngineTime { get; set; }
    //Total distance in meters
    [JsonPropertyName("totalDistance")]
    public double? TotalDistance { get; set; }
    //ml/100km
    [JsonPropertyName("avgFuelConsumption")]
    public double? AvgFuelConsumption { get; set; }
    //Wh/100km
    [JsonPropertyName("avgElectricEnergyConsumption")]
    public double? AvgElectricEnergyConsumption { get; set; }
    [JsonPropertyName("co2Emissions")]
    public double? Co2Emissions { get; set; }
    [JsonPropertyName("co2Saved")]
    public double? Co2Saved { get; set; }
}

public class UtilizationVehicle
{
    public string? Vin { get; set; }
    [JsonPropertyName("avgSpeedDriving")]
    public double? AvgSpeed { get; set; }
    //Total time the engine has been running in seconds
    [JsonPropertyName("totalTime")]
    public int TotalEngineTime { get; set; }
    //Total distance in meters
    [JsonPropertyName("totalDistance")]
    public double? TotalDistance { get; set; }
    //ml/100km
    [JsonPropertyName("avgFuelConsumption")]
    public double? AvgFuelConsumption { get; set; }
    //Wh/100km
    [JsonPropertyName("avgElectricEnergyConsumption")]
    public double? AvgElectricEnergyConsumption { get; set; }
    [JsonPropertyName("co2Emissions")]
    public double? Co2Emissions { get; set; }
    [JsonPropertyName("co2Saved")]
    public double? Co2Saved { get; set; }

}

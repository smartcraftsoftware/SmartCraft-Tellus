using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class ScaniaVehicleStatusResponse
{
    public bool MoreDataAvailable { get; set; }
    [JsonPropertyName("vehicleStatusResponse")]
    public ScaniaVehicleStatus VehicleStatusResponse { get; set; } = new();
}

public class ScaniaVehicleStatus
{
    [JsonPropertyName("vehicleStatuses")]
    public VehicleStatus[] VehicleStatuses { get; set; } = [];
}

public class VehicleStatus
{
    [JsonPropertyName("vin")]
    public string? Vin { get; set; }

    [JsonPropertyName("createdDateTime")]
    public DateTime? CreatedDateTime { get; set; }

    [JsonPropertyName("receivedDateTime")]
    public DateTime? ReceivedDateTime { get; set; }

    [JsonPropertyName("hrTotalVehicleDistance")]
    public int? HrTotalVehicleDistance { get; set; }

    [JsonPropertyName("totalEngineHours")]
    public double? TotalEngineHours { get; set; }

    [JsonPropertyName("totalElectricMotorHours")]
    public double? TotalElectricMotorHours { get; set; }

    [JsonPropertyName("totalFuelUsedGaseous")]
    public double? TotalGaseousFuelUsed { get; set; }
    public double? TotalElectricEnergyUsed { get; set; }

    [JsonPropertyName("engineTotalFuelUsed")]
    public int? EngineTotalFuelUsed { get; set; }
    public AccumulatedData? AccumulatedData { get; set; }
    public SnapShotData? SnapShotData { get; set; }

}
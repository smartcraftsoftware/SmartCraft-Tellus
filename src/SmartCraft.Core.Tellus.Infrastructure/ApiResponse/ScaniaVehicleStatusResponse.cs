using System.Text.Json.Serialization;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class VehicleStatusApiResponse
{
    [JsonPropertyName("vehicleStatusResponse")]
    public ScaniaVehicleStatusResponse? VehicleStatusResponse { get; set; }
}

public class ScaniaVehicleStatusResponse
{
    [JsonPropertyName("vehicleStatuses")]
    public VehicleStatus[]? VehicleStatuses { get; set; }
}

public class VehicleStatus
{
    [JsonPropertyName("vin")]
    public string? Vin { get; set; }

    [JsonPropertyName("createdDateTime")]
    public string? CreatedDateTime { get; set; }

    [JsonPropertyName("receivedDateTime")]
    public string? ReceivedDateTime { get; set; }

    [JsonPropertyName("hrTotalVehicleDistance")]
    public int? HrTotalVehicleDistance { get; set; }

    [JsonPropertyName("totalEngineHours")]
    public double? TotalEngineHours { get; set; }

    [JsonPropertyName("totalElectricMotorHours")]
    public double? TotalElectricMotorHours { get; set; }

    [JsonPropertyName("engineTotalFuelUsed")]
    public int? EngineTotalFuelUsed { get; set; }
}
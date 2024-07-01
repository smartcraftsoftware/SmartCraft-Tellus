namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class VolvoVehicleStatusResponse
{
    public bool MoreDataAvailable { get; set; }
    public VolvoVehicleStatus[]? VehicleStatus { get; set; }
}

public class VolvoVehicleStatus
{
    public required string Vin { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? ReceivedDateTime { get; set; }
    public int? HrTotalVehicleDistance { get; set; }
    public double? TotalEngineHours { get; set; }
    public double? TotalElectricMotorHours { get; set; }
    public double? EngineTotalFuelUsed { get; set; }
    public double? TotalGaseousFuelUsed { get; set; }
    public double? TotalElectricEnergyUsed { get; set; }
    public AccumulatedData? AccumulatedData { get; set; }
    public SnapShotData? SnapShotData { get; set; }
}

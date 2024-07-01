namespace SmartCraft.Core.Tellus.Api.Contracts.Responses;

public class IntervalStatusReportResponse
{
    public string? Vin { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public double? HrTotalVehicleDistance { get; set; }
    public double? TotalEngineHours { get; set; } 
    public double? TotalElectricMotorHours { get; set; }
    public double? EngineTotalFuelUsed { get; set; }
    public double? TotalGaseousFuelUsed { get; set; }
    public double? TotalElectricEnergyUsed { get; set; }
}
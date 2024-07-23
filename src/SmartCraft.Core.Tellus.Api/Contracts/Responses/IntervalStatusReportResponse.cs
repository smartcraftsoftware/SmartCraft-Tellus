namespace SmartCraft.Core.Tellus.Api.Contracts.Responses;

public class IntervalStatusReportResponse
{
    public string? Vin { get; set; }
    /// <summary>
    /// Start time of interval
    /// </summary>
    public DateTime? StartDateTime { get; set; }
    /// <summary>
    /// Stop time of interval
    /// </summary>
    public DateTime? EndDateTime { get; set; }
    /// <summary>
    /// Accumulated distance travelled by the vehicle during its operation in meter
    /// </summary>
    public double? HrTotalVehicleDistance { get; set; }
    /// <summary>
    /// The total hours of operation for the vehicle engine.
    /// </summary>
    public double? TotalEngineHours { get; set; }
    /// <summary>
    /// The total hours the electric motor is ready for propulsion
    /// </summary>
    public double? TotalElectricMotorHours { get; set; }
    /// <summary>
    /// Total fuel usage within the interval. In millilitres.
    /// </summary>
    public double? EngineTotalFuelUsed { get; set; }
    /// <summary>
    /// Total fuel consumed in kg (trip drive fuel + trip PTO governor moving
    /// fuel + trip PTO governor non-moving fuel + trip idle fuel)
    /// </summary>
    public double? TotalGaseousFuelUsed { get; set; }
    /// <summary>
    /// Total electric energy consumed by the vehicle, excluding when  plugged
    /// in (vehicle coupler) for charging, (incl.motor, PTO, cooling, etc.)
    /// in watt hours.Recuperation is subtracted from the value.
    /// </summary>
    public double? TotalElectricEnergyUsed { get; set; }
}
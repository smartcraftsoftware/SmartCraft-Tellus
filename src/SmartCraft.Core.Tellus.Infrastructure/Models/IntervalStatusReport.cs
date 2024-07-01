using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCraft.Core.Tellus.Infrastructure.Models;
public class IntervalStatusReport : BaseDbModel
{
    public Guid Id { get; set; }
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

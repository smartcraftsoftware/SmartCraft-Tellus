using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCraft.Core.Tellus.Infrastructure.Models;
public class VehicleProductionDate : BaseDbModel
{
    public Guid Id { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

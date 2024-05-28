using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCraft.Core.Tellus.Infrastructure.Models;
public class BaseDbModel
{
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid LastUpdatedBy { get; set; }
}

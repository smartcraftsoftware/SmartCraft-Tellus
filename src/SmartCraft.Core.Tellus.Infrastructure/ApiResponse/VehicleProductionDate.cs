using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class VehicleProductionDate
{
    [JsonPropertyName("day")]
    public int Day { get; set; }
    [JsonPropertyName("month")]
    public int Month { get; set; }
    [JsonPropertyName("year")]
    public int Year { get; set; }
}

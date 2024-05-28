namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class ManPerformApiResponse
{
    public string? StartTime { get; set; }
    public string? StopTime { get; set; }
    public ManPerformItem[]? Items { get; set; }
}

public class ManPerformItem
{
    public double? AvgSpeed { get; set; }
    //Co2Emissions in kg
    public double? Co2Emissions { get; set; }
    //Operational consumption in liters
    public double? FuelConsumption { get; set; }
    //Distance in km
    public double? Mileage { get; set; }

}



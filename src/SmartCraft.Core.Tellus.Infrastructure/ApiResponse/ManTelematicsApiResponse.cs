namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class ManTelematicsApiResponse
{
    public string? AssetId { get; set; }
    public ManTelematicsItem[]? Items { get; set; }
}

public class ManTelematicsItem
{
    public ManStateResponse? State { get; set; }
}

public class ManStateResponse
{
    //Mileage from odometer in KM
    public double? Mileage { get; set; }
    public bool Ignition { get; set; }
    //Fuel level in percentage
    public double? FuelLevel { get; set; }
    //Battery charge in percentage
    public double? StateOfCharge { get; set; }
    //Accumulated amount of fuel used during truck operation (l)
    public double? FuelConsumption { get; set; }

}

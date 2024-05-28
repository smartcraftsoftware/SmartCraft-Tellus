namespace SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
public class ManAssetApiResponse
{
    public ManAssetItem[]? Items { get; set; }
}

public class ManAssetItem
{
    public string? Id { get; set; }
    public string? Identification { get; set; }
    public string? IdentificationType { get; set; }
    public string? Brand { get; set; }
    public string? LicensePlate { get; set; }
    public Embedded? Embedded { get; set; }
    
}
public class Embedded
{
    public ManAssetMasterData? MasterData { get; set; }
}

public class ManAssetMasterData
{
    public string? FuelType { get; set; }
    public string? EngineType { get; set; }
}

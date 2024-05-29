namespace SmartCraft.Core.Tellus.Api.Contracts.Responses;

public class GetTenantResponse
{
    public string? DaimlerToken { get; set; }
    public string? VolvoCredentials { get; set; }
    public string? ScaniaSecretKey { get; set; }
    public string? ScaniaClientId { get; set; }
    public string? ManToken { get; set; }
}

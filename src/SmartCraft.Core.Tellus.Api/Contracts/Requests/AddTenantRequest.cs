namespace SmartCraft.Core.Tellus.Api.Contracts.Requests;
public class AddTenantRequest
{
    public string? VolvoCredentials { get; set; }
    public string? ScaniaClientId { get; set; }
    public string? ScaniaSecretKey { get; set; }
    public string? ManToken { get; set; }
    public string? DaimlerToken { get; set; }
}

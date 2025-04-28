namespace SmartCraft.Core.Tellus.Api.Contracts.Requests;

public class UpdateCompanyRequest
{
    public string? Name { get; set; }
    public string? VolvoCredentials { get; set; }
    public string? ScaniaClientId { get; set; }
    public string? ScaniaSecretKey { get; set; }
    public string? ManToken { get; set; }
    public string? DaimlerToken { get; set; }
}

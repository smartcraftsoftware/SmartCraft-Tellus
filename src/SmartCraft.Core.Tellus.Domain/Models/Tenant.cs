namespace SmartCraft.Core.Tellus.Domain.Models;
public class Tenant
{
    public Guid Id { get; set; }
    public string? VolvoCredentials { get; set; }
    public string? ScaniaClientId { get; set; }
    public string? ScaniaSecretKey { get; set; }
    public string? ManToken { get; set; }
    public string? DaimlerToken { get; set; }
}
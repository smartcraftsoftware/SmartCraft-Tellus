namespace SmartCraft.Core.Tellus.Infrastructure.Models;
public class Tenant : BaseDbModel
{
    public Guid Id { get; set; }
    public string? VolvoCredentials { get; set; }
    public string? ScaniaClientId { get; set; }
    public string? ScaniaSecretKey { get; set; }
    public string? ManToken { get; set; }
    public string? DaimlerToken { get; set; }
}

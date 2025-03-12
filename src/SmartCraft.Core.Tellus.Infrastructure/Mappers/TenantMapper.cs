namespace SmartCraft.Core.Tellus.Infrastructure.Mappers;
public static class TenantMapper
{
    public static Domain.Models.Company ToDomainModel(this Infrastructure.Models.Company tenant)
    {
        return new Domain.Models.Company
        {
            Id = tenant.Id,
            VolvoCredentials = tenant.VolvoCredentials,
            ScaniaSecretKey = tenant.ScaniaSecretKey,
            ScaniaClientId = tenant.ScaniaClientId,
            ManToken = tenant.ManToken,
            DaimlerToken = tenant.DaimlerToken

        };
    }

    public static Infrastructure.Models.Company ToDataModel(this Domain.Models.Company tenant)
    {
        return new Infrastructure.Models.Company
        {
            Id = tenant.Id,
            VolvoCredentials = tenant.VolvoCredentials,
            ScaniaClientId = tenant.ScaniaClientId,
            ScaniaSecretKey = tenant.ScaniaSecretKey,
            ManToken = tenant.ManToken,
            DaimlerToken = tenant.DaimlerToken
        };
    }

    public static Infrastructure.Models.Company ToCreateTenantModel(this Domain.Models.Company tenant, Guid tenantId)
    {
        return new Infrastructure.Models.Company
        {
            Id = tenantId,
            VolvoCredentials = tenant.VolvoCredentials,
            ScaniaClientId = tenant.ScaniaClientId,
            ScaniaSecretKey = tenant.ScaniaSecretKey,
        };
    }
}

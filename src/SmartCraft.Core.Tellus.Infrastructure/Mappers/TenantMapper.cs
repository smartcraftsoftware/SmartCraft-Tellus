namespace SmartCraft.Core.Tellus.Infrastructure.Mappers;
public static class TenantMapper
{
    public static Domain.Models.Tenant ToDomainModel(this Infrastructure.Models.Tenant tenant)
    {
        return new Domain.Models.Tenant
        {
            Id = tenant.Id,
            VolvoCredentials = tenant.VolvoCredentials,
            ScaniaSecretKey = tenant.ScaniaSecretKey,
            ScaniaClientId = tenant.ScaniaClientId,
            ManToken = tenant.ManToken,
            DaimlerToken = tenant.DaimlerToken

        };
    }

    public static Infrastructure.Models.Tenant ToDataModel(this Domain.Models.Tenant tenant)
    {
        return new Infrastructure.Models.Tenant
        {
            Id = tenant.Id,
            VolvoCredentials = tenant.VolvoCredentials,
            ScaniaClientId = tenant.ScaniaClientId,
            ScaniaSecretKey = tenant.ScaniaSecretKey,
            ManToken = tenant.ManToken,
            DaimlerToken = tenant.DaimlerToken
        };
    }

    public static Infrastructure.Models.Tenant ToCreateTenantModel(this Domain.Models.Tenant tenant, Guid tenantId)
    {
        return new Infrastructure.Models.Tenant
        {
            Id = tenantId,
            VolvoCredentials = tenant.VolvoCredentials,
            ScaniaClientId = tenant.ScaniaClientId,
            ScaniaSecretKey = tenant.ScaniaSecretKey,
        };
    }
}

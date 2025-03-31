namespace SmartCraft.Core.Tellus.Infrastructure.Mappers;
public static class TenantMapper
{
    public static Domain.Models.Company ToDomainModel(this Infrastructure.Models.Company company)
    {
        return new Domain.Models.Company
        {
            Id = company.Id,
            TenantId = company.TenantId,
            VolvoCredentials = company.VolvoCredentials,
            ScaniaSecretKey = company.ScaniaSecretKey,
            ScaniaClientId = company.ScaniaClientId,
            ManToken = company.ManToken,
            DaimlerToken = company.DaimlerToken

        };
    }

    public static Infrastructure.Models.Company ToDataModel(this Domain.Models.Company company)
    {
        return new Infrastructure.Models.Company
        {
            Id = company.Id,
            TenantId = company.TenantId,
            VolvoCredentials = company.VolvoCredentials,
            ScaniaClientId = company.ScaniaClientId,
            ScaniaSecretKey = company.ScaniaSecretKey,
            ManToken = company.ManToken,
            DaimlerToken = company.DaimlerToken
        };
    }

    public static Infrastructure.Models.Company ToCreateCompanyModel(this Domain.Models.Company company, Guid tenantId)
    {
        return new Infrastructure.Models.Company
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            VolvoCredentials = company.VolvoCredentials,
            ScaniaClientId = company.ScaniaClientId,
            ScaniaSecretKey = company.ScaniaSecretKey,
            DaimlerToken = company.DaimlerToken,
            ManToken = company.ManToken
        };
    }
}

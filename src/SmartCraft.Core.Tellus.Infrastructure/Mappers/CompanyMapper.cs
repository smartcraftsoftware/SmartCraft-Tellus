using SmartCraft.Core.Tellus.Infrastructure.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Mappers;
public static class CompanyMapper
{
    public static Domain.Models.Company ToDomainModel(this Company company)
    {
        return new Domain.Models.Company
        {
            Id = company.Id,
            TenantId = company.TenantId,
            Name = company.Name,
            VolvoCredentials = company.VolvoCredentials,
            ScaniaSecretKey = company.ScaniaSecretKey,
            ScaniaClientId = company.ScaniaClientId,
            ManToken = company.ManToken,
            DaimlerToken = company.DaimlerToken

        };
    }

    public static Company ToDataModel(this Domain.Models.Company company)
    {
        return new Company
        {
            Id = company.Id,
            TenantId = company.TenantId,
            Name = company.Name,
            VolvoCredentials = company.VolvoCredentials,
            ScaniaClientId = company.ScaniaClientId,
            ScaniaSecretKey = company.ScaniaSecretKey,
            ManToken = company.ManToken,
            DaimlerToken = company.DaimlerToken
        };
    }

    public static Company ToCreateCompanyModel(this Domain.Models.Company company, Guid tenantId)
    {
        return new Company
        {
            Id = Guid.NewGuid(),
            Name = company.Name,
            TenantId = tenantId,
            VolvoCredentials = company.VolvoCredentials,
            ScaniaClientId = company.ScaniaClientId,
            ScaniaSecretKey = company.ScaniaSecretKey,
            DaimlerToken = company.DaimlerToken,
            ManToken = company.ManToken
        };
    }
}

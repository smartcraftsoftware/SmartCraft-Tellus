
using SmartCraft.Core.Tellus.Api.Contracts.Requests;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;

namespace SmartCraft.Core.Tellus.Api.Mappers;
public static class CompanyMapper
{
    public static GetCompanyResponse ToResponseContract(this Domain.Models.Company company)
    {
        return new GetCompanyResponse
        {
            TenantId = company.TenantId,
            DaimlerToken = company.DaimlerToken,
            ManToken = company.ManToken,
            ScaniaSecretKey = company.ScaniaSecretKey,
            ScaniaClientId = company.ScaniaClientId,
            VolvoCredentials = company.VolvoCredentials
        };
    }

    public static Domain.Models.Company ToDomainModel(this AddCompanyRequest companyRequest)
    {
        return new Domain.Models.Company
        {
            TenantId = companyRequest.TenantId,
            DaimlerToken = companyRequest.DaimlerToken,
            ManToken = companyRequest.ManToken,
            ScaniaSecretKey = companyRequest.ScaniaSecretKey,
            ScaniaClientId = companyRequest.ScaniaClientId,
            VolvoCredentials = companyRequest.VolvoCredentials
        };
    }

    public static Domain.Models.Company ToDomainModel(this UpdateCompanyRequest companyRequest)
    {
        return new Domain.Models.Company
        {
            TenantId = companyRequest.TenantId,
            DaimlerToken = companyRequest.DaimlerToken,
            ManToken = companyRequest.ManToken,
            ScaniaSecretKey = companyRequest.ScaniaSecretKey,
            ScaniaClientId = companyRequest.ScaniaClientId,
            VolvoCredentials = companyRequest.VolvoCredentials
        };
    }
}


using SmartCraft.Core.Tellus.Api.Contracts.Requests;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;

namespace SmartCraft.Core.Tellus.Api.Mappers;
public static class CompanyMapper
{
    public static GetCompanyResponse ToResponseContract(this Domain.Models.Company company)
    {
        return new GetCompanyResponse
        {
            CompanyId = company.Id,
            TenantId = company.TenantId,
            Name = company.Name,
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
            Name = companyRequest.Name,
            TenantId = companyRequest.TenantId,
            DaimlerToken = companyRequest.DaimlerToken,
            ManToken = companyRequest.ManToken,
            ScaniaSecretKey = companyRequest.ScaniaSecretKey,
            ScaniaClientId = companyRequest.ScaniaClientId,
            VolvoCredentials = companyRequest.VolvoCredentials
        };
    }

    public static Domain.Models.Company ToDomainModel(this UpdateCompanyRequest companyRequest, Guid companyId)
    {
        return new Domain.Models.Company
        {
            Id = companyId,
            Name = companyRequest.Name,
            DaimlerToken = companyRequest.DaimlerToken,
            ManToken = companyRequest.ManToken,
            ScaniaSecretKey = companyRequest.ScaniaSecretKey,
            ScaniaClientId = companyRequest.ScaniaClientId,
            VolvoCredentials = companyRequest.VolvoCredentials
        };
    }
}

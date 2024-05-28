
using SmartCraft.Core.Tellus.Api.Contracts.Requests;
using SmartCraft.Core.Tellus.Api.Contracts.Responses;

namespace SmartCraft.Core.Tellus.Api.Mappers;
public static class TenantMapper
{
    public static GetTenantResponse ToResponseContract(this Domain.Models.Tenant tenant)
    {
        return new GetTenantResponse
        {
            DaimlerToken = tenant.DaimlerToken,
            ManToken = tenant.ManToken,
            ScaniaSecretKey = tenant.ScaniaSecretKey,
            ScaniaClientId = tenant.ScaniaClientId,
            VolvoCredentials = tenant.VolvoCredentials
        };
    }

    public static Domain.Models.Tenant ToDomainModel(this AddTenantRequest tenantRequest)
    {
        return new Domain.Models.Tenant
        {
            DaimlerToken = tenantRequest.DaimlerToken,
            ManToken = tenantRequest.ManToken,
            ScaniaSecretKey = tenantRequest.ScaniaSecretKey,
            ScaniaClientId = tenantRequest.ScaniaClientId,
            VolvoCredentials = tenantRequest.VolvoCredentials
        };
    }

    public static Domain.Models.Tenant ToDomainModel(this UpdateTenantRequest tenantRequest)
    {
        return new Domain.Models.Tenant
        {
            DaimlerToken = tenantRequest.DaimlerToken,
            ManToken = tenantRequest.ManToken,
            ScaniaSecretKey = tenantRequest.ScaniaSecretKey,
            ScaniaClientId = tenantRequest.ScaniaClientId,
            VolvoCredentials = tenantRequest.VolvoCredentials
        };
    }
}

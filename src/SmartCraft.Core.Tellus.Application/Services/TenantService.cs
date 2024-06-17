using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Domain.Services;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;

namespace SmartCraft.Core.Tellus.Application.Services;

public class TenantService(IRepository<Infrastructure.Models.Tenant, TenantContext> repository) : ITenantService
{
    public async Task<Tenant?> GetTenantAsync(Guid id)
    {
        var tenant = await repository.Get(id);
        if (tenant == null)
            return null;
        return tenant.ToDomainModel();
    }

    public async Task<List<Tenant>> GetTenantsAsync()
    {
        var tenants = await repository.GetAll();
        return tenants.Select(x => x.ToDomainModel()).ToList();
    }

    public async Task<Guid> RegisterTenantAsync(Guid tenantId, Tenant tenant)
    {
        var tenantToAdd = tenant.ToCreateTenantModel(tenantId);
        await repository.Add(tenantToAdd, tenantId);
        return tenantToAdd.Id;
    }

    public async Task<Tenant> UpdateTenantAsync(Guid id, Tenant tenant)
    {
        var existingTenant = await repository.Get(id);
        if (existingTenant == null)
        {
            throw new NullReferenceException("Tenant not found");
        }

        if (tenant.DaimlerToken != null)
            existingTenant.DaimlerToken = tenant.DaimlerToken;
        if (tenant.ManToken != null)
            existingTenant.ManToken = tenant.ManToken;
        if (tenant.ScaniaClientId != null)
            existingTenant.ScaniaClientId = tenant.ScaniaClientId;
        if (tenant.ScaniaSecretKey != null)
            existingTenant.ScaniaSecretKey = tenant.ScaniaSecretKey;
        if (tenant.VolvoCredentials != null)
            existingTenant.VolvoCredentials = tenant.VolvoCredentials;

        var updatedTenant = await repository.Update(existingTenant, existingTenant.Id);

        return updatedTenant.ToDomainModel();
    }

    public async Task<bool> DeleteTenant(Guid id)
    {
       return await repository.Delete(id);
    }
}


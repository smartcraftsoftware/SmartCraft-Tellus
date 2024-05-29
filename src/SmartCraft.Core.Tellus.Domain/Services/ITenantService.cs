using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Domain.Services;

public interface ITenantService
{
	Task<Guid> RegisterTenantAsync(Guid tenantId, Tenant tenant);
	Task<Tenant?> GetTenantAsync(Guid id);
	Task<List<Tenant>> GetTenantsAsync();
	Task<Tenant> UpdateTenantAsync(Guid id, Tenant tenant);
	Task<bool> DeleteTenant(Guid id);
}

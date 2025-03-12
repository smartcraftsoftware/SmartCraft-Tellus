using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Domain.Services;

public interface ITenantService
{
	Task<Guid> RegisterTenantAsync(Guid tenantId, Company tenant);
	Task<Company?> GetTenantAsync(Guid id);
	Task<List<Company>> GetTenantsAsync();
	Task<Company> UpdateTenantAsync(Guid id, Company tenant);
	Task<bool> DeleteTenant(Guid id);
}

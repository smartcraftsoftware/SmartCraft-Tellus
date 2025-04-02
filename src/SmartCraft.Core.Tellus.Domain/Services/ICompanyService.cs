using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Domain.Services;

public interface ICompanyService
{
	Task<Guid> RegisterCompanyAsync(Guid tenantId, Company company);
	Task<Company?> GetCompanyAsync(Guid companyId, Guid tenantId);
    Task<List<Company>> GetCompaniesAsync(Guid tenantId);
	Task<Company> UpdateCompanyAsync(Company company);
    Task<bool> DeleteCompany(Guid id);
}

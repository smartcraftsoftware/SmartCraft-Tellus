using Microsoft.EntityFrameworkCore;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Repositories;

public class CompanyRepository : Repository<Company, CompanyContext>, ICompanyRepository<Company, CompanyContext>
{
    public CompanyRepository(CompanyContext context) : base(context)
    {
    }

    public async Task<List<Company>> GetAll(Guid tenantId)
    {
        return await _entities
            .Where(company => company.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<Company> Get(Guid tenantId, Guid companyId)
    {
        return await _entities.Where(t => t.TenantId == tenantId && t.Id == companyId).SingleOrDefaultAsync();
    }
}

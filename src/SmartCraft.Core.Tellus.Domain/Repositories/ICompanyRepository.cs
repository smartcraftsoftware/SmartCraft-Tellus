using Microsoft.EntityFrameworkCore;

namespace SmartCraft.Core.Tellus.Domain.Repositories;

public interface ICompanyRepository<TEntity, TContext> : IRepository<TEntity, TContext> where TEntity : class where TContext : DbContext
{
    Task<List<TEntity>> GetAll(Guid tenantId);
    Task<TEntity> Get(Guid tenantId, Guid companyId);
}


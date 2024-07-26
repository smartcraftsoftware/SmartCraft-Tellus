using Microsoft.EntityFrameworkCore;

namespace SmartCraft.Core.Tellus.Domain.Repositories;

public interface IRepository<TEntity, TContext> where TEntity : class where TContext : DbContext
{
    Task Add(TEntity entity, Guid tenantId);
    Task<List<TEntity>> GetAll();
    Task<TEntity?> Get(Guid id);
    Task<TEntity> Update(TEntity entity, Guid tenantId);
    Task<bool> Delete(Guid id);
}

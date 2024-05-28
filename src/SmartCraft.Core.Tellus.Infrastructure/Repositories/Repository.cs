using Microsoft.EntityFrameworkCore;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Infrastructure.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Repositories;

public class Repository<TEntity, TContext> : IRepository<TEntity, TContext> where TEntity : BaseDbModel where TContext : DbContext
{
    private readonly DbSet<TEntity> _entities;
    private readonly TContext _context;
 
    public Repository(TContext context)
    {
        _context = context;
        _entities = context.Set<TEntity>();
    }
    public async Task Add(TEntity entity, Guid tenantId)
    {
        try 
        {
            entity.CreatedAt = DateTime.Now;
            entity.LastUpdated = DateTime.Now;
            entity.CreatedBy = tenantId;
            entity.LastUpdatedBy = tenantId;
            _entities.Add(entity);
            var ok = await _context.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            throw new Exception("Could not add entity", ex);
        }
    } 

    public async Task<List<TEntity>> GetAll()
    {
        return await _entities.ToListAsync();
    }

    public async Task<TEntity> Get(Guid id)
    {
        #pragma warning disable CS8603
        return await _entities.FindAsync(id);
        #pragma warning restore CS8603
    }

    public async Task<TEntity> Update(TEntity entity, Guid tenantId)
    {
        entity.LastUpdated = DateTime.Now;
        entity.LastUpdatedBy = tenantId;
        _context.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _entities.FindAsync(id);
        if(entity == null)
            return false;

        _entities.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}

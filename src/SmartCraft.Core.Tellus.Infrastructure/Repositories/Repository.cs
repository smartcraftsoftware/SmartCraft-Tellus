using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Infrastructure.Models;

namespace SmartCraft.Core.Tellus.Infrastructure.Repositories;

public class Repository<TEntity, TContext> : IRepository<TEntity, TContext> where TEntity : BaseDbModel where TContext : DbContext
{
    private readonly DbSet<TEntity> _entities;
    private readonly TContext _context;
    private readonly ILogger _logger;
 
    public Repository(TContext context, ILogger logger)
    {
        _context = context;
        _entities = context.Set<TEntity>();
        _logger = SetLoggerContext(logger);
    }
    public async Task Add(TEntity entity, Guid tenantId)
    {
        try 
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.LastUpdated = DateTime.UtcNow;
            entity.CreatedBy = tenantId;
            entity.LastUpdatedBy = tenantId;
            _entities.Add(entity);
            var ok = await _context.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            _logger.Error("Trying to add {Entity} did not succeed with {Message}", entity, ex.Message);
            throw new Exception("Could not add entity", ex);
        }
    } 

    public async Task<List<TEntity>> GetAll()
    {
            return await _entities.ToListAsync();
    }

    public async Task<TEntity?> Get(Guid id)
    {
        #pragma warning disable CS8603
        return await _entities.FindAsync(id);
        #pragma warning restore CS8603
    }

    public async Task<TEntity> Update(TEntity entity, Guid tenantId)
    {
        try
        {
            entity.LastUpdated = DateTime.UtcNow;
            entity.LastUpdatedBy = tenantId;
            _context.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex) 
        {
            _logger.Error("Unable to update {Entity} with {ErrorMessage}", entity, ex.Message);
            throw new Exception("Could not update entity");
        }

    }

    public async Task<bool> Delete(Guid id)
    {
        try
        {
            var entity = await _entities.FindAsync(id);
            if (entity == null)
                return false;

            _entities.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex) 
        {
            _logger.Error("Could not delete Entity with {Id} with {ErrorMessage}", id, ex.Message);
        }

        return false;
    }

    private ILogger SetLoggerContext(ILogger logger)
    {
        return logger.ForContext(typeof(Repository<,>));
    }

}

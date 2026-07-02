using Arquanix.Domain.Core;
using Arquanix.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Arquanix.Infrastructure.Core;


public abstract class BaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly ArquanixDbContext Context;

    protected BaseRepository(ArquanixDbContext context)
    {
        Context = context;
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await Context.Set<TEntity>().OrderBy(e => e.Id).ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await Context.Set<TEntity>().FindAsync(id);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await Context.Set<TEntity>().AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> UpdateAsync(TEntity entity)
    {
        if (!await ExistsAsync(entity.Id))
        {
            return false;
        }

        Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await Context.Set<TEntity>().FindAsync(id);
        if (entity is null)
        {
            return false;
        }

        Context.Set<TEntity>().Remove(entity);
        await Context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await Context.Set<TEntity>().AnyAsync(e => e.Id == id);
    }
}

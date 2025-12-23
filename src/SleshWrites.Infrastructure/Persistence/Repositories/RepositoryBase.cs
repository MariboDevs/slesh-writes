using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Infrastructure.Persistence.Repositories;

/// <summary>
/// Base repository implementation with common CRUD operations.
/// </summary>
public abstract class RepositoryBase<T> : IRepository<T> where T : Entity
{
    protected readonly SleshWritesDbContext Context;
    protected readonly DbSet<T> DbSet;

    protected RepositoryBase(SleshWritesDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public virtual void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        DbSet.Remove(entity);
    }
}

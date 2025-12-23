using Microsoft.EntityFrameworkCore;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Category entity.
/// </summary>
public sealed class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
{
    public CategoryRepository(SleshWritesDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(c => c.Slug.Value == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllOrderedAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(c => c.Slug.Value == slug);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasBlogPostsAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await Context.BlogPosts
            .AnyAsync(bp => bp.CategoryId == categoryId, cancellationToken);
    }
}

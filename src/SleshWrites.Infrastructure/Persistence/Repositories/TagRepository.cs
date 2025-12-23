using Microsoft.EntityFrameworkCore;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Enums;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Tag entity.
/// </summary>
public sealed class TagRepository : RepositoryBase<Tag>, ITagRepository
{
    public TagRepository(SleshWritesDbContext context) : base(context)
    {
    }

    public async Task<Tag?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(t => t.Slug.Value == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<Tag>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return await DbSet
            .Where(t => idList.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Tag>> GetPopularTagsAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        // Get tags ordered by the number of published blog posts using them
        return await Context.BlogPosts
            .Where(bp => bp.Status == PostStatus.Published)
            .SelectMany(bp => bp.Tags)
            .GroupBy(t => t.Id)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.First())
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(t => t.Slug.Value == slug);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Enums;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for BlogPost aggregate root.
/// </summary>
public sealed class BlogPostRepository : RepositoryBase<BlogPost>, IBlogPostRepository
{
    public BlogPostRepository(SleshWritesDbContext context) : base(context)
    {
    }

    public async Task<BlogPost?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(bp => bp.Tags)
            .Include(bp => bp.Category)
            .FirstOrDefaultAsync(bp => bp.Slug.Value == slug, cancellationToken);
    }

    public async Task<BlogPost?> GetByIdWithTagsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(bp => bp.Tags)
            .FirstOrDefaultAsync(bp => bp.Id == id, cancellationToken);
    }

    public async Task<PagedResult<BlogPost>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        PostStatus? status = null,
        Guid? categoryId = null,
        Guid? authorId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(bp => bp.Tags)
            .Include(bp => bp.Category)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(bp => bp.Status == status.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(bp => bp.CategoryId == categoryId.Value);
        }

        if (authorId.HasValue)
        {
            query = query.Where(bp => bp.AuthorId == authorId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(bp => bp.PublishedAt ?? bp.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BlogPost>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<BlogPost>> GetByTagAsync(
        Guid tagId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(bp => bp.Tags)
            .Include(bp => bp.Category)
            .Where(bp => bp.Tags.Any(t => t.Id == tagId) && bp.Status == PostStatus.Published);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(bp => bp.PublishedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BlogPost>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<IReadOnlyList<BlogPost>> GetRecentPublishedAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(bp => bp.Tags)
            .Include(bp => bp.Category)
            .Where(bp => bp.Status == PostStatus.Published)
            .OrderByDescending(bp => bp.PublishedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(bp => bp.Slug.Value == slug);

        if (excludeId.HasValue)
        {
            query = query.Where(bp => bp.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}

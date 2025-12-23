using SleshWrites.Domain.Common;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Enums;

namespace SleshWrites.Domain.Repositories;

/// <summary>
/// Repository interface for BlogPost aggregate root.
/// </summary>
public interface IBlogPostRepository : IRepository<BlogPost>
{
    Task<BlogPost?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<BlogPost?> GetByIdWithTagsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<BlogPost>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        PostStatus? status = null,
        Guid? categoryId = null,
        Guid? authorId = null,
        CancellationToken cancellationToken = default);
    Task<PagedResult<BlogPost>> GetByTagAsync(
        Guid tagId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BlogPost>> GetRecentPublishedAsync(
        int count,
        CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);
}

using SleshWrites.Domain.Entities;

namespace SleshWrites.Domain.Repositories;

/// <summary>
/// Repository interface for Tag entity.
/// </summary>
public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tag>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tag>> GetPopularTagsAsync(int count, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);
}

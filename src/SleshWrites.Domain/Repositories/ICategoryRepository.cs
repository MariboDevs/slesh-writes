using SleshWrites.Domain.Entities;

namespace SleshWrites.Domain.Repositories;

/// <summary>
/// Repository interface for Category entity.
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasBlogPostsAsync(Guid categoryId, CancellationToken cancellationToken = default);
}

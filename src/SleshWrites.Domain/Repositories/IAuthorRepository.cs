using SleshWrites.Domain.Entities;

namespace SleshWrites.Domain.Repositories;

/// <summary>
/// Repository interface for Author entity.
/// </summary>
public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByAzureAdB2CIdAsync(string azureAdB2CId, CancellationToken cancellationToken = default);
    Task<bool> AzureAdB2CIdExistsAsync(string azureAdB2CId, CancellationToken cancellationToken = default);
    Task<bool> HasBlogPostsAsync(Guid authorId, CancellationToken cancellationToken = default);
}

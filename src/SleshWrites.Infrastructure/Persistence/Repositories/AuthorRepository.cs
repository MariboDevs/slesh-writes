using Microsoft.EntityFrameworkCore;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Author entity.
/// </summary>
public sealed class AuthorRepository : RepositoryBase<Author>, IAuthorRepository
{
    public AuthorRepository(SleshWritesDbContext context) : base(context)
    {
    }

    public async Task<Author?> GetByAzureAdB2CIdAsync(
        string azureAdB2CId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(a => a.AzureAdB2CId == azureAdB2CId, cancellationToken);
    }

    public async Task<bool> AzureAdB2CIdExistsAsync(
        string azureAdB2CId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(a => a.AzureAdB2CId == azureAdB2CId, cancellationToken);
    }

    public async Task<bool> HasBlogPostsAsync(
        Guid authorId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<BlogPost>()
            .AnyAsync(bp => bp.AuthorId == authorId, cancellationToken);
    }
}

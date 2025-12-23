using SleshWrites.Domain.Common;

namespace SleshWrites.Domain.Repositories;

/// <summary>
/// Unit of Work interface for coordinating repository operations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IBlogPostRepository BlogPosts { get; }
    ICategoryRepository Categories { get; }
    ITagRepository Tags { get; }
    IAuthorRepository Authors { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Result> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<Result> CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task<Result> RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

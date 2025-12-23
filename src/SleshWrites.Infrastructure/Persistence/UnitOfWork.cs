using Microsoft.EntityFrameworkCore.Storage;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for coordinating repository operations.
/// Repositories are injected via DI to follow Dependency Inversion Principle.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SleshWritesDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public IBlogPostRepository BlogPosts { get; }
    public ICategoryRepository Categories { get; }
    public ITagRepository Tags { get; }
    public IAuthorRepository Authors { get; }

    public UnitOfWork(
        SleshWritesDbContext context,
        IBlogPostRepository blogPosts,
        ICategoryRepository categories,
        ITagRepository tags,
        IAuthorRepository authors)
    {
        _context = context;
        BlogPosts = blogPosts;
        Categories = categories;
        Tags = tags;
        Authors = authors;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            return Result.Failure("A transaction is already in progress.");
        }

        try
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to begin transaction: {ex.Message}");
        }
    }

    public async Task<Result> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return Result.Failure("No transaction has been started.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await RollbackTransactionAsync(cancellationToken);
            return Result.Failure($"Transaction commit failed: {ex.Message}");
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task<Result> RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return Result.Success(); // Nothing to rollback
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Transaction rollback failed: {ex.Message}");
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _currentTransaction?.Dispose();
        _context.Dispose();
        _disposed = true;
    }
}

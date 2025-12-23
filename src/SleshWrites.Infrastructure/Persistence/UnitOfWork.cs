using Microsoft.EntityFrameworkCore.Storage;
using SleshWrites.Domain.Repositories;
using SleshWrites.Infrastructure.Persistence.Repositories;

namespace SleshWrites.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for coordinating repository operations.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SleshWritesDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    private IBlogPostRepository? _blogPosts;
    private ICategoryRepository? _categories;
    private ITagRepository? _tags;
    private IAuthorRepository? _authors;

    public UnitOfWork(SleshWritesDbContext context)
    {
        _context = context;
    }

    public IBlogPostRepository BlogPosts =>
        _blogPosts ??= new BlogPostRepository(_context);

    public ICategoryRepository Categories =>
        _categories ??= new CategoryRepository(_context);

    public ITagRepository Tags =>
        _tags ??= new TagRepository(_context);

    public IAuthorRepository Authors =>
        _authors ??= new AuthorRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            return;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            throw new InvalidOperationException("No transaction has been started.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
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

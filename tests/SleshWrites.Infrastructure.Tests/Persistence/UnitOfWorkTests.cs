using FluentAssertions;
using SleshWrites.Domain.Entities;
using SleshWrites.Infrastructure.Persistence;
using SleshWrites.Infrastructure.Persistence.Repositories;

namespace SleshWrites.Infrastructure.Tests.Persistence;

public class UnitOfWorkTests : IDisposable
{
    private readonly SleshWritesDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        _context = TestDbContextFactory.Create();

        // Create repositories for the UnitOfWork
        var blogPostRepository = new BlogPostRepository(_context);
        var categoryRepository = new CategoryRepository(_context);
        var tagRepository = new TagRepository(_context);
        var authorRepository = new AuthorRepository(_context);

        _unitOfWork = new UnitOfWork(
            _context,
            blogPostRepository,
            categoryRepository,
            tagRepository,
            authorRepository);
    }

    [Fact]
    public void BlogPosts_ReturnsRepository()
    {
        // Act
        var repository = _unitOfWork.BlogPosts;

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public void Categories_ReturnsRepository()
    {
        // Act
        var repository = _unitOfWork.Categories;

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public void Tags_ReturnsRepository()
    {
        // Act
        var repository = _unitOfWork.Tags;

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public void Authors_ReturnsRepository()
    {
        // Act
        var repository = _unitOfWork.Authors;

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsChanges()
    {
        // Arrange
        var author = Author.Create("azure-id", "Test Author").Value;
        _unitOfWork.Authors.Add(author);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(1);

        var savedAuthor = await _unitOfWork.Authors.GetByIdAsync(author.Id);
        savedAuthor.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleEntities_PersistsAll()
    {
        // Arrange
        var author = Author.Create("azure-id", "Test Author").Value;
        var category = Category.Create("Technology").Value;
        var tag = Tag.Create("CSharp").Value;

        _unitOfWork.Authors.Add(author);
        _unitOfWork.Categories.Add(category);
        _unitOfWork.Tags.Add(tag);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        // The count may be higher than 3 due to owned entities (Slug) being tracked separately by EF Core
        result.Should().BeGreaterThanOrEqualTo(3);

        // Verify entities were actually saved
        var savedAuthor = await _unitOfWork.Authors.GetByIdAsync(author.Id);
        var savedCategory = await _unitOfWork.Categories.GetByIdAsync(category.Id);
        var savedTag = await _unitOfWork.Tags.GetByIdAsync(tag.Id);

        savedAuthor.Should().NotBeNull();
        savedCategory.Should().NotBeNull();
        savedTag.Should().NotBeNull();
    }

    [Fact]
    public async Task BeginTransactionAsync_ReturnsSuccess()
    {
        // Act
        var result = await _unitOfWork.BeginTransactionAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task BeginTransactionAsync_WhenAlreadyStarted_ReturnsFailure()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Act
        var result = await _unitOfWork.BeginTransactionAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already in progress");
    }

    [Fact]
    public async Task CommitTransactionAsync_WithoutTransaction_ReturnsFailure()
    {
        // Act
        var result = await _unitOfWork.CommitTransactionAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("No transaction");
    }

    [Fact]
    public async Task RollbackTransactionAsync_WithoutTransaction_ReturnsSuccess()
    {
        // Act
        var result = await _unitOfWork.RollbackTransactionAsync();

        // Assert - rollback of nothing is considered success
        result.IsSuccess.Should().BeTrue();
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
    }
}

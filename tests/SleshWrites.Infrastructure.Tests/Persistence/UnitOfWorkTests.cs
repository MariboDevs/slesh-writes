using FluentAssertions;
using SleshWrites.Domain.Entities;
using SleshWrites.Infrastructure.Persistence;

namespace SleshWrites.Infrastructure.Tests.Persistence;

public class UnitOfWorkTests : IDisposable
{
    private readonly SleshWritesDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        _context = TestDbContextFactory.Create();
        _unitOfWork = new UnitOfWork(_context);
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
    public void Repositories_AreCached()
    {
        // Act
        var blogPosts1 = _unitOfWork.BlogPosts;
        var blogPosts2 = _unitOfWork.BlogPosts;

        // Assert
        blogPosts1.Should().BeSameAs(blogPosts2);
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
    }
}

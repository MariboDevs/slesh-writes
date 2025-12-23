using FluentAssertions;
using SleshWrites.Domain.Entities;
using SleshWrites.Infrastructure.Persistence;
using SleshWrites.Infrastructure.Persistence.Repositories;

namespace SleshWrites.Infrastructure.Tests.Persistence.Repositories;

public class CategoryRepositoryTests : IDisposable
{
    private readonly SleshWritesDbContext _context;
    private readonly CategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _repository = new CategoryRepository(_context);
    }

    [Fact]
    public async Task Add_AndGetById_ReturnsCategory()
    {
        // Arrange
        var category = Category.Create("Technology", "Tech related posts", 1).Value;

        // Act
        _repository.Add(category);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(category.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Technology");
        result.Slug.Value.Should().Be("technology");
    }

    [Fact]
    public async Task GetBySlugAsync_WithExistingSlug_ReturnsCategory()
    {
        // Arrange
        var category = Category.Create("Software Development").Value;
        _repository.Add(category);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySlugAsync("software-development");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Software Development");
    }

    [Fact]
    public async Task GetAllOrderedAsync_ReturnsCategoriesOrderedByDisplayOrder()
    {
        // Arrange
        var category1 = Category.Create("Alpha", displayOrder: 3).Value;
        var category2 = Category.Create("Beta", displayOrder: 1).Value;
        var category3 = Category.Create("Gamma", displayOrder: 2).Value;

        _repository.Add(category1);
        _repository.Add(category2);
        _repository.Add(category3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllOrderedAsync();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Beta");
        result[1].Name.Should().Be("Gamma");
        result[2].Name.Should().Be("Alpha");
    }

    [Fact]
    public async Task SlugExistsAsync_WithExistingSlug_ReturnsTrue()
    {
        // Arrange
        var category = Category.Create("Existing Category").Value;
        _repository.Add(category);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SlugExistsAsync("existing-category");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SlugExistsAsync_WithExcludeId_ExcludesSpecifiedCategory()
    {
        // Arrange
        var category = Category.Create("Test Category").Value;
        _repository.Add(category);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SlugExistsAsync("test-category", excludeId: category.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasBlogPostsAsync_WithAssociatedPosts_ReturnsTrue()
    {
        // Arrange
        var category = Category.Create("Category With Posts").Value;
        _repository.Add(category);

        var blogPost = BlogPost.Create(
            "Test Post",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            category.Id).Value;

        _context.BlogPosts.Add(blogPost);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasBlogPostsAsync(category.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasBlogPostsAsync_WithNoAssociatedPosts_ReturnsFalse()
    {
        // Arrange
        var category = Category.Create("Empty Category").Value;
        _repository.Add(category);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasBlogPostsAsync(category.Id);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

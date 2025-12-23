using FluentAssertions;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Enums;
using SleshWrites.Infrastructure.Persistence;
using SleshWrites.Infrastructure.Persistence.Repositories;

namespace SleshWrites.Infrastructure.Tests.Persistence.Repositories;

public class BlogPostRepositoryTests : IDisposable
{
    private readonly SleshWritesDbContext _context;
    private readonly BlogPostRepository _repository;

    public BlogPostRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _repository = new BlogPostRepository(_context);
    }

    [Fact]
    public async Task Add_AndGetById_ReturnsBlogPost()
    {
        // Arrange
        var blogPost = BlogPost.Create(
            "Test Post",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            Guid.NewGuid()).Value;

        // Act
        _repository.Add(blogPost);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(blogPost.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Post");
        result.Slug.Value.Should().Be("test-post");
    }

    [Fact]
    public async Task GetBySlugAsync_WithExistingSlug_ReturnsBlogPost()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Technology").Value;
        _context.Categories.Add(category);

        var blogPost = BlogPost.Create(
            "Unique Post Title",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            category.Id).Value;

        _repository.Add(blogPost);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySlugAsync("unique-post-title");

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Unique Post Title");
    }

    [Fact]
    public async Task GetBySlugAsync_WithNonExistentSlug_ReturnsNull()
    {
        // Act
        var result = await _repository.GetBySlugAsync("non-existent-slug");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithTagsAsync_ReturnsBlogPostWithTags()
    {
        // Arrange
        var tag1 = Tag.Create("CSharp").Value;
        var tag2 = Tag.Create("DotNet").Value;
        _context.Tags.AddRange(tag1, tag2);

        var blogPost = BlogPost.Create(
            "Post With Tags",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            Guid.NewGuid()).Value;

        blogPost.AddTag(tag1);
        blogPost.AddTag(tag2);

        _repository.Add(blogPost);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithTagsAsync(blogPost.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Tags.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResults()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var category = Category.Create("Test Category").Value;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        for (var i = 1; i <= 15; i++)
        {
            var post = BlogPost.Create(
                $"Post {i}",
                $"This is the content of post {i}. It needs to be at least 50 characters long.",
                authorId,
                category.Id).Value;

            _repository.Add(post);
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetPagedAsync(1, 10);

        // Assert
        result.Items.Should().HaveCount(10);
        result.TotalCount.Should().Be(15);
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetPagedAsync_WithStatusFilter_ReturnsOnlyMatchingPosts()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var category = Category.Create("Test Category").Value;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var draftPost = BlogPost.Create(
            "Draft Post",
            "This is the content of a draft post. It needs to be at least 50 characters.",
            authorId,
            category.Id).Value;

        var publishedPost = BlogPost.Create(
            "Published Post",
            "This is the content of a published post. It needs to be at least 50 characters.",
            authorId,
            category.Id).Value;
        publishedPost.Publish();

        _repository.Add(draftPost);
        _repository.Add(publishedPost);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetPagedAsync(1, 10, status: PostStatus.Published);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].Title.Should().Be("Published Post");
    }

    [Fact]
    public async Task GetRecentPublishedAsync_ReturnsOnlyPublishedPostsOrderedByDate()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var category = Category.Create("Test Category").Value;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var draftPost = BlogPost.Create(
            "Draft Post",
            "This is the content of a draft post. It needs to be at least 50 characters.",
            authorId,
            category.Id).Value;

        var publishedPost1 = BlogPost.Create(
            "Published Post 1",
            "This is the content of published post 1. It needs to be at least 50 characters.",
            authorId,
            category.Id).Value;
        publishedPost1.Publish();

        var publishedPost2 = BlogPost.Create(
            "Published Post 2",
            "This is the content of published post 2. It needs to be at least 50 characters.",
            authorId,
            category.Id).Value;
        publishedPost2.Publish();

        _repository.Add(draftPost);
        _repository.Add(publishedPost1);
        _repository.Add(publishedPost2);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetRecentPublishedAsync(5);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Status == PostStatus.Published);
    }

    [Fact]
    public async Task SlugExistsAsync_WithExistingSlug_ReturnsTrue()
    {
        // Arrange
        var blogPost = BlogPost.Create(
            "Existing Post",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            Guid.NewGuid()).Value;

        _repository.Add(blogPost);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SlugExistsAsync("existing-post");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SlugExistsAsync_WithExcludeId_ExcludesSpecifiedPost()
    {
        // Arrange
        var blogPost = BlogPost.Create(
            "Existing Post",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            Guid.NewGuid()).Value;

        _repository.Add(blogPost);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SlugExistsAsync("existing-post", excludeId: blogPost.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Update_ModifiesBlogPost()
    {
        // Arrange
        var blogPost = BlogPost.Create(
            "Original Title",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            Guid.NewGuid()).Value;

        _repository.Add(blogPost);
        await _context.SaveChangesAsync();

        // Act
        blogPost.UpdateContent(
            "Updated Title",
            "This is the updated content of the blog post. It needs to be at least 50 characters.");
        _repository.Update(blogPost);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(blogPost.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task Remove_DeletesBlogPost()
    {
        // Arrange
        var blogPost = BlogPost.Create(
            "Post To Delete",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            Guid.NewGuid()).Value;

        _repository.Add(blogPost);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(blogPost);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(blogPost.Id);

        // Assert
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

using FluentAssertions;
using SleshWrites.Domain.Entities;
using SleshWrites.Infrastructure.Persistence;
using SleshWrites.Infrastructure.Persistence.Repositories;

namespace SleshWrites.Infrastructure.Tests.Persistence.Repositories;

public class TagRepositoryTests : IDisposable
{
    private readonly SleshWritesDbContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _repository = new TagRepository(_context);
    }

    [Fact]
    public async Task Add_AndGetById_ReturnsTag()
    {
        // Arrange
        var tag = Tag.Create("CSharp").Value;

        // Act
        _repository.Add(tag);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(tag.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("CSharp");
        result.Slug.Value.Should().Be("csharp");
    }

    [Fact]
    public async Task GetBySlugAsync_WithExistingSlug_ReturnsTag()
    {
        // Arrange
        var tag = Tag.Create("Entity Framework").Value;
        _repository.Add(tag);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySlugAsync("entity-framework");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Entity Framework");
    }

    [Fact]
    public async Task GetByIdsAsync_ReturnsMatchingTags()
    {
        // Arrange
        var tag1 = Tag.Create("Tag1").Value;
        var tag2 = Tag.Create("Tag2").Value;
        var tag3 = Tag.Create("Tag3").Value;

        _repository.Add(tag1);
        _repository.Add(tag2);
        _repository.Add(tag3);
        await _context.SaveChangesAsync();

        var idsToFind = new List<Guid> { tag1.Id, tag3.Id };

        // Act
        var result = await _repository.GetByIdsAsync(idsToFind);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Id == tag1.Id);
        result.Should().Contain(t => t.Id == tag3.Id);
    }

    [Fact]
    public async Task GetPopularTagsAsync_ReturnsTagsOrderedByUsageCount()
    {
        // Arrange
        var tag1 = Tag.Create("Popular").Value;
        var tag2 = Tag.Create("LessPopular").Value;
        var tag3 = Tag.Create("Unpopular").Value;

        _repository.Add(tag1);
        _repository.Add(tag2);
        _repository.Add(tag3);

        // Create published posts with tags
        for (var i = 0; i < 5; i++)
        {
            var post = BlogPost.Create(
                $"Post with Popular Tag {i}",
                "This is the content of the test blog post. It needs to be at least 50 characters.",
                Guid.NewGuid(),
                Guid.NewGuid()).Value;
            post.AddTag(tag1);
            post.Publish();
            _context.BlogPosts.Add(post);
        }

        for (var i = 0; i < 2; i++)
        {
            var post = BlogPost.Create(
                $"Post with Less Popular Tag {i}",
                "This is the content of the test blog post. It needs to be at least 50 characters.",
                Guid.NewGuid(),
                Guid.NewGuid()).Value;
            post.AddTag(tag2);
            post.Publish();
            _context.BlogPosts.Add(post);
        }

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPopularTagsAsync(2);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Popular");
        result[1].Name.Should().Be("LessPopular");
    }

    [Fact]
    public async Task SlugExistsAsync_WithExistingSlug_ReturnsTrue()
    {
        // Arrange
        var tag = Tag.Create("Existing Tag").Value;
        _repository.Add(tag);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SlugExistsAsync("existing-tag");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SlugExistsAsync_WithExcludeId_ExcludesSpecifiedTag()
    {
        // Arrange
        var tag = Tag.Create("Test Tag").Value;
        _repository.Add(tag);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SlugExistsAsync("test-tag", excludeId: tag.Id);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

using FluentAssertions;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Enums;
using SleshWrites.Domain.Events;

namespace SleshWrites.Domain.Tests.Entities;

public class BlogPostTests
{
    private readonly Guid _validAuthorId = Guid.NewGuid();
    private readonly Guid _validCategoryId = Guid.NewGuid();
    private const string ValidTitle = "My First Blog Post";
    private const string ValidContent = "This is the content of my first blog post. It needs to be at least 50 characters long.";

    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Act
        var result = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be(ValidTitle);
        result.Value.Content.Should().Be(ValidContent);
        result.Value.AuthorId.Should().Be(_validAuthorId);
        result.Value.CategoryId.Should().Be(_validCategoryId);
        result.Value.Status.Should().Be(PostStatus.Draft);
        result.Value.Slug.Value.Should().Be("my-first-blog-post");
    }

    [Fact]
    public void Create_WithValidData_RaisesBlogPostCreatedEvent()
    {
        // Act
        var result = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId);

        // Assert
        result.Value.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BlogPostCreatedEvent>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_ReturnsFailure(string? title)
    {
        // Act
        var result = BlogPost.Create(title!, ValidContent, _validAuthorId, _validCategoryId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Title cannot be empty");
    }

    [Fact]
    public void Create_WithTooLongTitle_ReturnsFailure()
    {
        // Arrange
        var longTitle = new string('a', 201);

        // Act
        var result = BlogPost.Create(longTitle, ValidContent, _validAuthorId, _validCategoryId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("200 characters");
    }

    [Fact]
    public void Create_WithTooShortContent_ReturnsFailure()
    {
        // Act
        var result = BlogPost.Create(ValidTitle, "Short", _validAuthorId, _validCategoryId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("at least 50 characters");
    }

    [Fact]
    public void Create_WithEmptyAuthorId_ReturnsFailure()
    {
        // Act
        var result = BlogPost.Create(ValidTitle, ValidContent, Guid.Empty, _validCategoryId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Author ID cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyCategoryId_ReturnsFailure()
    {
        // Act
        var result = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Category ID cannot be empty");
    }

    [Fact]
    public void Create_WithExcerpt_TruncatesLongExcerpt()
    {
        // Arrange
        var longExcerpt = new string('a', 600);

        // Act
        var result = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId, longExcerpt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Excerpt.Should().HaveLength(500);
    }

    [Fact]
    public void Publish_FromDraft_ReturnsSuccess()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        post.ClearDomainEvents();

        // Act
        var result = post.Publish();

        // Assert
        result.IsSuccess.Should().BeTrue();
        post.Status.Should().Be(PostStatus.Published);
        post.PublishedAt.Should().NotBeNull();
        post.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BlogPostPublishedEvent>();
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_ReturnsFailure()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        post.Publish();

        // Act
        var result = post.Publish();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already published");
    }

    [Fact]
    public void Publish_WhenArchived_ReturnsFailure()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        post.Publish();
        post.Archive();

        // Act
        var result = post.Publish();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cannot publish an archived");
    }

    [Fact]
    public void Archive_WhenPublished_ReturnsSuccess()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        post.Publish();

        // Act
        var result = post.Archive();

        // Assert
        result.IsSuccess.Should().BeTrue();
        post.Status.Should().Be(PostStatus.Archived);
    }

    [Fact]
    public void Archive_WhenAlreadyArchived_ReturnsFailure()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        post.Archive();

        // Act
        var result = post.Archive();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already archived");
    }

    [Fact]
    public void Unarchive_WhenArchived_ReturnsSuccess()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        post.Archive();

        // Act
        var result = post.Unarchive();

        // Assert
        result.IsSuccess.Should().BeTrue();
        post.Status.Should().Be(PostStatus.Draft);
    }

    [Fact]
    public void Unarchive_WhenNotArchived_ReturnsFailure()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;

        // Act
        var result = post.Unarchive();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not archived");
    }

    [Fact]
    public void UpdateContent_WithValidData_UpdatesTitleAndContent()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        var newTitle = "Updated Title";
        var newContent = "This is the updated content for the blog post that is long enough.";

        // Act
        var result = post.UpdateContent(newTitle, newContent);

        // Assert
        result.IsSuccess.Should().BeTrue();
        post.Title.Should().Be(newTitle);
        post.Content.Should().Be(newContent);
        post.Slug.Value.Should().Be("updated-title");
    }

    [Fact]
    public void UpdateContent_WhenPublished_DoesNotUpdateSlug()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        var originalSlug = post.Slug.Value;
        post.Publish();

        // Act
        var result = post.UpdateContent("New Title", ValidContent);

        // Assert
        result.IsSuccess.Should().BeTrue();
        post.Title.Should().Be("New Title");
        post.Slug.Value.Should().Be(originalSlug);
    }

    [Fact]
    public void SetFeaturedImage_WithValidUrl_ReturnsSuccess()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        var imageUrl = "https://example.com/image.jpg";

        // Act
        var result = post.SetFeaturedImage(imageUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        post.FeaturedImage.Should().Be(imageUrl);
    }

    [Fact]
    public void SetFeaturedImage_WithInvalidUrl_ReturnsFailure()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;

        // Act
        var result = post.SetFeaturedImage("not-a-valid-url");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("valid absolute URL");
    }

    [Fact]
    public void SetFeaturedImage_WithNull_ClearsImage()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        post.SetFeaturedImage("https://example.com/image.jpg");

        // Act
        var result = post.SetFeaturedImage(null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        post.FeaturedImage.Should().BeNull();
    }

    [Fact]
    public void AddTag_WithNewTag_AddsTagToCollection()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        var tag = Tag.Create("Technology").Value;

        // Act
        post.AddTag(tag);

        // Assert
        post.Tags.Should().ContainSingle().Which.Should().Be(tag);
    }

    [Fact]
    public void AddTag_WithDuplicateTag_DoesNotAddAgain()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        var tag = Tag.Create("Technology").Value;
        post.AddTag(tag);

        // Act
        post.AddTag(tag);

        // Assert
        post.Tags.Should().ContainSingle();
    }

    [Fact]
    public void RemoveTag_WithExistingTag_RemovesFromCollection()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        var tag = Tag.Create("Technology").Value;
        post.AddTag(tag);

        // Act
        post.RemoveTag(tag.Id);

        // Assert
        post.Tags.Should().BeEmpty();
    }

    [Fact]
    public void ClearTags_RemovesAllTags()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        post.AddTag(Tag.Create("Tag1").Value);
        post.AddTag(Tag.Create("Tag2").Value);

        // Act
        post.ClearTags();

        // Assert
        post.Tags.Should().BeEmpty();
    }

    [Fact]
    public void SetTags_ReplacesExistingTags()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        post.AddTag(Tag.Create("OldTag").Value);
        var newTags = new[] { Tag.Create("NewTag1").Value, Tag.Create("NewTag2").Value };

        // Act
        post.SetTags(newTags);

        // Assert
        post.Tags.Should().HaveCount(2);
        post.Tags.Select(t => t.Name).Should().BeEquivalentTo("NewTag1", "NewTag2");
    }

    [Fact]
    public void UpdateCategory_WithValidId_UpdatesCategoryId()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;
        var newCategoryId = Guid.NewGuid();

        // Act
        var result = post.UpdateCategory(newCategoryId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        post.CategoryId.Should().Be(newCategoryId);
    }

    [Fact]
    public void UpdateCategory_WithEmptyId_ReturnsFailure()
    {
        // Arrange
        var post = BlogPost.Create(ValidTitle, ValidContent, _validAuthorId, _validCategoryId).Value;

        // Act
        var result = post.UpdateCategory(Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Category ID cannot be empty");
        post.CategoryId.Should().Be(_validCategoryId);
    }
}

using FluentAssertions;
using SleshWrites.Domain.ValueObjects;

namespace SleshWrites.Domain.Tests.ValueObjects;

public class PostMetaDataTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var title = "SEO Title";
        var description = "SEO Description";
        var keywords = "keyword1, keyword2";
        var canonicalUrl = "https://example.com/post";
        var ogImage = "https://example.com/image.jpg";

        // Act
        var result = PostMetaData.Create(title, description, keywords, canonicalUrl, ogImage);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be(title);
        result.Value.Description.Should().Be(description);
        result.Value.Keywords.Should().Be(keywords);
        result.Value.CanonicalUrl.Should().Be(canonicalUrl);
        result.Value.OgImage.Should().Be(ogImage);
    }

    [Fact]
    public void Create_WithNullValues_ReturnsSuccessWithNulls()
    {
        // Act
        var result = PostMetaData.Create(null, null, null, null, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().BeNull();
        result.Value.Description.Should().BeNull();
        result.Value.Keywords.Should().BeNull();
    }

    [Fact]
    public void Create_WithTooLongTitle_ReturnsFailure()
    {
        // Arrange
        var longTitle = new string('a', 61);

        // Act
        var result = PostMetaData.Create(longTitle, null, null, null, null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("60 characters");
    }

    [Fact]
    public void Create_WithTooLongDescription_ReturnsFailure()
    {
        // Arrange
        var longDescription = new string('a', 161);

        // Act
        var result = PostMetaData.Create(null, longDescription, null, null, null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("160 characters");
    }

    [Fact]
    public void Create_WithTooLongKeywords_ReturnsFailure()
    {
        // Arrange
        var longKeywords = new string('a', 201);

        // Act
        var result = PostMetaData.Create(null, null, longKeywords, null, null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("200 characters");
    }

    [Fact]
    public void Create_WithInvalidCanonicalUrl_ReturnsFailure()
    {
        // Act
        var result = PostMetaData.Create(null, null, null, "not-a-valid-url", null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("valid");
    }

    [Fact]
    public void Create_WithInvalidOgImage_ReturnsFailure()
    {
        // Act
        var result = PostMetaData.Create(null, null, null, null, "invalid-image-url");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("valid");
    }

    [Fact]
    public void Empty_ReturnsEmptyMetaData()
    {
        // Act
        var metaData = PostMetaData.Empty();

        // Assert
        metaData.Title.Should().BeNull();
        metaData.Description.Should().BeNull();
        metaData.Keywords.Should().BeNull();
        metaData.CanonicalUrl.Should().BeNull();
        metaData.OgImage.Should().BeNull();
    }

    [Fact]
    public void Equality_SameMetaData_AreEqual()
    {
        // Arrange
        var meta1 = PostMetaData.Create("Title", "Desc", "key1, key2", null, null).Value;
        var meta2 = PostMetaData.Create("Title", "Desc", "key1, key2", null, null).Value;

        // Assert
        meta1.Should().Be(meta2);
    }

    [Fact]
    public void WithTitle_ReturnsNewInstanceWithUpdatedTitle()
    {
        // Arrange
        var original = PostMetaData.Create("Original", "Desc", null, null, null).Value;

        // Act
        var result = original.WithTitle("Updated");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Updated");
        result.Value.Description.Should().Be("Desc");
        original.Title.Should().Be("Original");
    }

    [Fact]
    public void WithTitle_WithInvalidTitle_ReturnsFailure()
    {
        // Arrange
        var original = PostMetaData.Create("Original", "Desc", null, null, null).Value;
        var longTitle = new string('a', 61);

        // Act
        var result = original.WithTitle(longTitle);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("60 characters");
    }

    [Fact]
    public void WithDescription_ReturnsNewInstanceWithUpdatedDescription()
    {
        // Arrange
        var original = PostMetaData.Create("Title", "Original", null, null, null).Value;

        // Act
        var result = original.WithDescription("Updated");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().Be("Updated");
        result.Value.Title.Should().Be("Title");
        original.Description.Should().Be("Original");
    }

    [Fact]
    public void WithDescription_WithInvalidDescription_ReturnsFailure()
    {
        // Arrange
        var original = PostMetaData.Create("Title", "Original", null, null, null).Value;
        var longDescription = new string('a', 161);

        // Act
        var result = original.WithDescription(longDescription);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("160 characters");
    }
}

using FluentAssertions;
using SleshWrites.Application.BlogPosts.Commands.CreateBlogPost;

namespace SleshWrites.Application.Tests.BlogPosts.Validators;

public class CreateBlogPostCommandValidatorTests
{
    private readonly CreateBlogPostCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ReturnsNoErrors()
    {
        // Arrange
        var command = new CreateBlogPostCommand(
            Title: "Valid Title",
            Content: "This is valid content that is definitely long enough to pass the 50 character minimum requirement.",
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ReturnsError()
    {
        // Arrange
        var command = new CreateBlogPostCommand(
            Title: "",
            Content: "This is valid content that is definitely long enough to pass the 50 character minimum requirement.",
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_WithTooLongTitle_ReturnsError()
    {
        // Arrange
        var command = new CreateBlogPostCommand(
            Title: new string('a', 201),
            Content: "This is valid content that is definitely long enough to pass the 50 character minimum requirement.",
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains("200"));
    }

    [Fact]
    public void Validate_WithTooShortContent_ReturnsError()
    {
        // Arrange
        var command = new CreateBlogPostCommand(
            Title: "Valid Title",
            Content: "Too short",
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Content" && e.ErrorMessage.Contains("50"));
    }

    [Fact]
    public void Validate_WithEmptyAuthorId_ReturnsError()
    {
        // Arrange
        var command = new CreateBlogPostCommand(
            Title: "Valid Title",
            Content: "This is valid content that is definitely long enough to pass the 50 character minimum requirement.",
            AuthorId: Guid.Empty,
            CategoryId: Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AuthorId");
    }

    [Fact]
    public void Validate_WithInvalidFeaturedImageUrl_ReturnsError()
    {
        // Arrange
        var command = new CreateBlogPostCommand(
            Title: "Valid Title",
            Content: "This is valid content that is definitely long enough to pass the 50 character minimum requirement.",
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid(),
            FeaturedImage: "not-a-valid-url");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FeaturedImage");
    }

    [Fact]
    public void Validate_WithValidFeaturedImageUrl_ReturnsNoErrors()
    {
        // Arrange
        var command = new CreateBlogPostCommand(
            Title: "Valid Title",
            Content: "This is valid content that is definitely long enough to pass the 50 character minimum requirement.",
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid(),
            FeaturedImage: "https://example.com/image.jpg");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

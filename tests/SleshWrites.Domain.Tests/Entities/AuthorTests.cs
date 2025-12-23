using FluentAssertions;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Domain.Tests.Entities;

public class AuthorTests
{
    private const string ValidAzureAdB2CId = "azure-ad-b2c-id-123";
    private const string ValidDisplayName = "John Doe";
    private const string ValidEmail = "john@example.com";

    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Act
        var result = Author.Create(ValidAzureAdB2CId, ValidDisplayName, ValidEmail);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AzureAdB2CId.Should().Be(ValidAzureAdB2CId);
        result.Value.DisplayName.Should().Be(ValidDisplayName);
        result.Value.Email.Should().Be(ValidEmail);
    }

    [Fact]
    public void Create_WithoutEmail_ReturnsSuccess()
    {
        // Act
        var result = Author.Create(ValidAzureAdB2CId, ValidDisplayName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyAzureAdB2CId_ReturnsFailure(string? azureId)
    {
        // Act
        var result = Author.Create(azureId!, ValidDisplayName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Azure AD B2C ID cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyDisplayName_ReturnsFailure(string? displayName)
    {
        // Act
        var result = Author.Create(ValidAzureAdB2CId, displayName!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Display name cannot be empty");
    }

    [Fact]
    public void Create_WithTooLongDisplayName_ReturnsFailure()
    {
        // Arrange
        var longName = new string('a', 101);

        // Act
        var result = Author.Create(ValidAzureAdB2CId, longName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("100 characters");
    }

    [Fact]
    public void Create_WithTooLongEmail_ReturnsFailure()
    {
        // Arrange
        var longEmail = new string('a', 257);

        // Act
        var result = Author.Create(ValidAzureAdB2CId, ValidDisplayName, longEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("256 characters");
    }

    [Fact]
    public void UpdateDisplayName_WithValidName_UpdatesDisplayName()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;

        // Act
        var result = author.UpdateDisplayName("Jane Doe");

        // Assert
        result.IsSuccess.Should().BeTrue();
        author.DisplayName.Should().Be("Jane Doe");
    }

    [Fact]
    public void UpdateDisplayName_WithEmptyName_ReturnsFailure()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;

        // Act
        var result = author.UpdateDisplayName("");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateBio_WithValidBio_UpdatesBio()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;

        // Act
        author.UpdateBio("This is my bio.");

        // Assert
        author.Bio.Should().Be("This is my bio.");
    }

    [Fact]
    public void UpdateBio_WithLongBio_TruncatesTo1000()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;
        var longBio = new string('a', 1500);

        // Act
        author.UpdateBio(longBio);

        // Assert
        author.Bio.Should().HaveLength(1000);
    }

    [Fact]
    public void UpdateAvatarUrl_WithValidUrl_ReturnsSuccess()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;
        var avatarUrl = "https://example.com/avatar.jpg";

        // Act
        var result = author.UpdateAvatarUrl(avatarUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        author.AvatarUrl.Should().Be(avatarUrl);
    }

    [Fact]
    public void UpdateAvatarUrl_WithInvalidUrl_ReturnsFailure()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;

        // Act
        var result = author.UpdateAvatarUrl("not-a-valid-url");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("valid absolute URL");
    }

    [Fact]
    public void AddSocialLink_WithValidData_AddsSocialLink()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;

        // Act
        author.AddSocialLink("Twitter", "https://twitter.com/johndoe");

        // Assert
        author.SocialLinks.Should().ContainKey("twitter");
        author.SocialLinks["twitter"].Should().Be("https://twitter.com/johndoe");
    }

    [Fact]
    public void AddSocialLink_WithEmptyPlatform_DoesNotAdd()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;

        // Act
        author.AddSocialLink("", "https://twitter.com/johndoe");

        // Assert
        author.SocialLinks.Should().BeEmpty();
    }

    [Fact]
    public void AddSocialLink_WithEmptyUrl_DoesNotAdd()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;

        // Act
        author.AddSocialLink("Twitter", "");

        // Assert
        author.SocialLinks.Should().BeEmpty();
    }

    [Fact]
    public void AddSocialLink_WithDuplicatePlatform_UpdatesUrl()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;
        author.AddSocialLink("Twitter", "https://twitter.com/johndoe");

        // Act
        author.AddSocialLink("Twitter", "https://twitter.com/janedoe");

        // Assert
        author.SocialLinks["twitter"].Should().Be("https://twitter.com/janedoe");
    }

    [Fact]
    public void RemoveSocialLink_WithExistingPlatform_RemovesLink()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;
        author.AddSocialLink("Twitter", "https://twitter.com/johndoe");

        // Act
        author.RemoveSocialLink("Twitter");

        // Assert
        author.SocialLinks.Should().BeEmpty();
    }

    [Fact]
    public void RemoveSocialLink_WithNonExistingPlatform_DoesNothing()
    {
        // Arrange
        var author = Author.Create(ValidAzureAdB2CId, ValidDisplayName).Value;
        author.AddSocialLink("Twitter", "https://twitter.com/johndoe");

        // Act
        author.RemoveSocialLink("LinkedIn");

        // Assert
        author.SocialLinks.Should().ContainSingle();
    }
}

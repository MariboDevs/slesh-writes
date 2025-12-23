using FluentAssertions;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Domain.Tests.Entities;

public class TagTests
{
    [Fact]
    public void Create_WithValidName_ReturnsSuccess()
    {
        // Act
        var result = Tag.Create("C# Programming");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("C# Programming");
        result.Value.Slug.Value.Should().Be("c-programming");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ReturnsFailure(string? name)
    {
        // Act
        var result = Tag.Create(name!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("cannot be empty");
    }

    [Fact]
    public void Create_WithTooLongName_ReturnsFailure()
    {
        // Arrange
        var longName = new string('a', 51);

        // Act
        var result = Tag.Create(longName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("50 characters");
    }

    [Fact]
    public void UpdateName_WithValidName_UpdatesNameAndSlug()
    {
        // Arrange
        var tag = Tag.Create("Technology").Value;

        // Act
        var result = tag.UpdateName("Programming");

        // Assert
        result.IsSuccess.Should().BeTrue();
        tag.Name.Should().Be("Programming");
        tag.Slug.Value.Should().Be("programming");
    }

    [Fact]
    public void UpdateName_WithEmptyName_ReturnsFailure()
    {
        // Arrange
        var tag = Tag.Create("Technology").Value;

        // Act
        var result = tag.UpdateName("");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateName_WithTooLongName_ReturnsFailure()
    {
        // Arrange
        var tag = Tag.Create("Technology").Value;
        var longName = new string('a', 51);

        // Act
        var result = tag.UpdateName(longName);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}

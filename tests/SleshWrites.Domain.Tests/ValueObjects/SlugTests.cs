using FluentAssertions;
using SleshWrites.Domain.ValueObjects;

namespace SleshWrites.Domain.Tests.ValueObjects;

public class SlugTests
{
    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("My First Blog Post", "my-first-blog-post")]
    [InlineData("C# Programming Tips", "c-programming-tips")]
    [InlineData("   Spaces   Around   ", "spaces-around")]
    [InlineData("Multiple---Dashes", "multiple-dashes")]
    [InlineData("UPPERCASE TITLE", "uppercase-title")]
    public void Create_WithValidInput_ReturnsSlug(string input, string expected)
    {
        // Act
        var result = Slug.Create(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ReturnsFailure(string? input)
    {
        // Act
        var result = Slug.Create(input!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("cannot be empty");
    }

    [Fact]
    public void Create_WithTooLongInput_ReturnsFailure()
    {
        // Arrange
        var longInput = new string('a', 201);

        // Act
        var result = Slug.Create(longInput);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("200 characters");
    }

    [Theory]
    [InlineData("Hello@World!", "helloworld")]
    [InlineData("Test & Sample", "test-sample")]
    [InlineData("Price: $100", "price-100")]
    public void Create_WithSpecialCharacters_RemovesThem(string input, string expected)
    {
        // Act
        var result = Slug.Create(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(expected);
    }

    [Fact]
    public void Equality_SameSlugs_AreEqual()
    {
        // Arrange
        var slug1 = Slug.Create("Hello World").Value;
        var slug2 = Slug.Create("Hello World").Value;

        // Assert
        slug1.Should().Be(slug2);
        (slug1 == slug2).Should().BeTrue();
    }

    [Fact]
    public void Equality_DifferentSlugs_AreNotEqual()
    {
        // Arrange
        var slug1 = Slug.Create("Hello World").Value;
        var slug2 = Slug.Create("Different Title").Value;

        // Assert
        slug1.Should().NotBe(slug2);
        (slug1 != slug2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ReturnsSlugValue()
    {
        // Arrange
        var slug = Slug.Create("Hello World").Value;

        // Act & Assert
        slug.ToString().Should().Be("hello-world");
    }

    [Fact]
    public void ImplicitConversion_ToString_Works()
    {
        // Arrange
        var slug = Slug.Create("Hello World").Value;

        // Act
        string stringValue = slug;

        // Assert
        stringValue.Should().Be("hello-world");
    }
}

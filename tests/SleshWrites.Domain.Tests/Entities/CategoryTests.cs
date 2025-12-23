using FluentAssertions;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Domain.Tests.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Act
        var result = Category.Create("Technology", "Tech articles");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Technology");
        result.Value.Description.Should().Be("Tech articles");
        result.Value.Slug.Value.Should().Be("technology");
        result.Value.DisplayOrder.Should().Be(0);
    }

    [Fact]
    public void Create_WithDisplayOrder_SetsDisplayOrder()
    {
        // Act
        var result = Category.Create("Technology", "Tech articles", 5);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DisplayOrder.Should().Be(5);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ReturnsFailure(string? name)
    {
        // Act
        var result = Category.Create(name!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("cannot be empty");
    }

    [Fact]
    public void Create_WithTooLongName_ReturnsFailure()
    {
        // Arrange
        var longName = new string('a', 101);

        // Act
        var result = Category.Create(longName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("100 characters");
    }

    [Fact]
    public void Create_WithTooLongDescription_ReturnsFailure()
    {
        // Arrange
        var longDescription = new string('a', 501);

        // Act
        var result = Category.Create("Technology", longDescription);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("500 characters");
    }

    [Fact]
    public void UpdateName_WithValidName_UpdatesNameAndSlug()
    {
        // Arrange
        var category = Category.Create("Technology").Value;

        // Act
        var result = category.UpdateName("Programming");

        // Assert
        result.IsSuccess.Should().BeTrue();
        category.Name.Should().Be("Programming");
        category.Slug.Value.Should().Be("programming");
    }

    [Fact]
    public void UpdateName_WithEmptyName_ReturnsFailure()
    {
        // Arrange
        var category = Category.Create("Technology").Value;

        // Act
        var result = category.UpdateName("");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_UpdatesDescription()
    {
        // Arrange
        var category = Category.Create("Technology").Value;

        // Act
        category.UpdateDescription("New description");

        // Assert
        category.Description.Should().Be("New description");
    }

    [Fact]
    public void UpdateDescription_WithLongDescription_TruncatesTo500()
    {
        // Arrange
        var category = Category.Create("Technology").Value;
        var longDescription = new string('a', 600);

        // Act
        category.UpdateDescription(longDescription);

        // Assert
        category.Description.Should().HaveLength(500);
    }

    [Fact]
    public void UpdateDisplayOrder_UpdatesDisplayOrder()
    {
        // Arrange
        var category = Category.Create("Technology").Value;

        // Act
        category.UpdateDisplayOrder(5);

        // Assert
        category.DisplayOrder.Should().Be(5);
    }
}

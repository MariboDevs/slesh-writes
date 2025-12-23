using FluentAssertions;
using SleshWrites.Domain.Common;

namespace SleshWrites.Domain.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ReturnsFailureResult()
    {
        // Act
        var result = Result.Failure("Error message");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Error message");
    }

    [Fact]
    public void SuccessT_ReturnsSuccessResultWithValue()
    {
        // Act
        var result = Result.Success(42);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void FailureT_ReturnsFailureResultWithError()
    {
        // Act
        var result = Result.Failure<int>("Error message");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Error message");
    }

    [Fact]
    public void Value_WhenFailure_ThrowsInvalidOperationException()
    {
        // Arrange
        var result = Result.Failure<int>("Error");

        // Act & Assert
        var act = () => result.Value;
        act.Should().Throw<InvalidOperationException>();
    }
}

public class PagedResultTests
{
    [Fact]
    public void Constructor_WithValidData_ReturnsPagedResult()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };

        // Act
        var result = new PagedResult<string>(items, 100, 1, 10);

        // Assert
        result.Items.Should().BeEquivalentTo(items);
        result.TotalCount.Should().Be(100);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(10);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void TotalPages_WithExactDivision_ReturnsCorrectCount()
    {
        // Arrange & Act
        var result = new PagedResult<string>([], 50, 1, 10);

        // Assert
        result.TotalPages.Should().Be(5);
    }

    [Fact]
    public void TotalPages_WithRemainder_RoundsUp()
    {
        // Arrange & Act
        var result = new PagedResult<string>([], 51, 1, 10);

        // Assert
        result.TotalPages.Should().Be(6);
    }

    [Fact]
    public void HasPreviousPage_WhenOnFirstPage_ReturnsFalse()
    {
        // Arrange & Act
        var result = new PagedResult<string>([], 50, 1, 10);

        // Assert
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_WhenNotOnFirstPage_ReturnsTrue()
    {
        // Arrange & Act
        var result = new PagedResult<string>([], 50, 2, 10);

        // Assert
        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_WhenOnLastPage_ReturnsFalse()
    {
        // Arrange & Act
        var result = new PagedResult<string>([], 50, 5, 10);

        // Assert
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_WhenNotOnLastPage_ReturnsTrue()
    {
        // Arrange & Act
        var result = new PagedResult<string>([], 50, 4, 10);

        // Assert
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void Empty_ReturnsEmptyPagedResult()
    {
        // Act
        var result = PagedResult<string>.Empty(1, 10);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(0);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }
}

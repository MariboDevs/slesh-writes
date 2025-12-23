using FluentAssertions;
using SleshWrites.API.Common;

namespace SleshWrites.API.Tests;

/// <summary>
/// Unit tests for ApiResponse wrappers.
/// </summary>
public class ApiResponseTests
{
    [Fact]
    public void GenericApiResponse_Ok_ReturnsSuccessWithData()
    {
        // Arrange
        var data = "test data";

        // Act
        var response = ApiResponse<string>.Ok(data);

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().Be(data);
        response.Error.Should().BeNull();
    }

    [Fact]
    public void GenericApiResponse_Fail_ReturnsFailureWithError()
    {
        // Arrange
        var error = "Something went wrong";

        // Act
        var response = ApiResponse<string>.Fail(error);

        // Assert
        response.Success.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Error.Should().Be(error);
    }

    [Fact]
    public void ApiResponse_Ok_ReturnsSuccess()
    {
        // Act
        var response = ApiResponse.Ok();

        // Assert
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();
    }

    [Fact]
    public void ApiResponse_Fail_ReturnsFailureWithError()
    {
        // Arrange
        var error = "Operation failed";

        // Act
        var response = ApiResponse.Fail(error);

        // Assert
        response.Success.Should().BeFalse();
        response.Error.Should().Be(error);
    }
}

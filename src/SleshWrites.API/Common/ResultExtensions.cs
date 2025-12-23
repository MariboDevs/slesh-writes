using SleshWrites.Domain.Common;

namespace SleshWrites.API.Common;

/// <summary>
/// Extension methods for converting Result types to IResult HTTP responses.
/// Eliminates duplication across all API endpoints.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result to OK (200) on success, or BadRequest (400) on failure.
    /// Use for queries that return collections or when entity existence is guaranteed.
    /// </summary>
    public static IResult ToOkResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? Results.Ok(ApiResponse<T>.Ok(result.Value))
            : Results.BadRequest(ApiResponse<T>.Fail(result.Error ?? "An error occurred."));

    /// <summary>
    /// Converts a Result to OK (200) on success, or NotFound (404) on failure.
    /// Use for queries that fetch a single entity by ID or slug.
    /// </summary>
    public static IResult ToNotFoundResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? Results.Ok(ApiResponse<T>.Ok(result.Value))
            : Results.NotFound(ApiResponse<T>.Fail(result.Error ?? "Resource not found."));

    /// <summary>
    /// Converts a Result to Created (201) on success, or BadRequest (400) on failure.
    /// Use for POST commands that create new resources.
    /// </summary>
    public static IResult ToCreatedResult<T>(this Result<T> result, string location) =>
        result.IsSuccess
            ? Results.Created(location, ApiResponse<T>.Ok(result.Value))
            : Results.BadRequest(ApiResponse<T>.Fail(result.Error ?? "Failed to create resource."));

    /// <summary>
    /// Converts a Result to NoContent (204) on success, or BadRequest (400) on failure.
    /// Use for PUT/PATCH commands that update existing resources.
    /// </summary>
    public static IResult ToNoContentResult(this Result result) =>
        result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(ApiResponse.Fail(result.Error ?? "Operation failed."));

    /// <summary>
    /// Converts a Result to NoContent (204) on success, or NotFound (404) on failure.
    /// Use for DELETE commands where the entity might not exist.
    /// </summary>
    public static IResult ToDeleteResult(this Result result) =>
        result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(ApiResponse.Fail(result.Error ?? "Resource not found."));
}

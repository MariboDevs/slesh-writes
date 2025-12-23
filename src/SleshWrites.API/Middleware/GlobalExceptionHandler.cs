using System.Net;
using System.Text.Json;
using FluentValidation;
using SleshWrites.API.Common;

namespace SleshWrites.API.Middleware;

/// <summary>
/// Global exception handling middleware for consistent error responses.
/// </summary>
public sealed class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationException => HandleValidationException(validationException),
            ArgumentException argumentException => HandleArgumentException(argumentException),
            KeyNotFoundException keyNotFoundException => HandleKeyNotFoundException(keyNotFoundException),
            UnauthorizedAccessException unauthorizedException => HandleUnauthorizedException(unauthorizedException),
            _ => HandleUnexpectedException(exception)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }

    private (HttpStatusCode, ApiResponse) HandleValidationException(ValidationException exception)
    {
        _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", exception.Errors.Select(e => e.ErrorMessage)));

        var errorMessage = string.Join("; ", exception.Errors.Select(e => e.ErrorMessage));
        return (HttpStatusCode.BadRequest, ApiResponse.Fail(errorMessage));
    }

    private (HttpStatusCode, ApiResponse) HandleArgumentException(ArgumentException exception)
    {
        _logger.LogWarning("Argument error: {Message}", exception.Message);
        return (HttpStatusCode.BadRequest, ApiResponse.Fail(exception.Message));
    }

    private (HttpStatusCode, ApiResponse) HandleKeyNotFoundException(KeyNotFoundException exception)
    {
        _logger.LogWarning("Resource not found: {Message}", exception.Message);
        return (HttpStatusCode.NotFound, ApiResponse.Fail(exception.Message));
    }

    private (HttpStatusCode, ApiResponse) HandleUnauthorizedException(UnauthorizedAccessException exception)
    {
        _logger.LogWarning("Unauthorized access: {Message}", exception.Message);
        return (HttpStatusCode.Unauthorized, ApiResponse.Fail("Unauthorized access."));
    }

    private (HttpStatusCode, ApiResponse) HandleUnexpectedException(Exception exception)
    {
        _logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);
        return (HttpStatusCode.InternalServerError, ApiResponse.Fail("An unexpected error occurred. Please try again later."));
    }
}

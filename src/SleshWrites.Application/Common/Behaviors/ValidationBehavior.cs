using FluentValidation;
using MediatR;
using SleshWrites.Domain.Common;

namespace SleshWrites.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates requests using FluentValidation.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        // Build error message from validation failures
        var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

        // Return appropriate Result type based on TResponse
        return CreateFailureResult<TResponse>(errorMessage);
    }

    private static T CreateFailureResult<T>(string error)
    {
        // Handle Result type
        if (typeof(T) == typeof(Result))
        {
            return (T)(object)Result.Failure(error);
        }

        // Handle Result<TValue> type
        if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(T).GetGenericArguments()[0];
            var failureMethod = typeof(Result)
                .GetMethod(nameof(Result.Failure), 1, [typeof(string)])!
                .MakeGenericMethod(valueType);

            return (T)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException($"Unsupported response type: {typeof(T).Name}");
    }
}

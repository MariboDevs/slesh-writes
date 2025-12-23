namespace SleshWrites.Domain.Common;

/// <summary>
/// Represents the result of an operation that may succeed or fail.
/// Use this instead of exceptions for business logic validation.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("A successful result cannot have an error message.");

        if (!isSuccess && string.IsNullOrWhiteSpace(error))
            throw new InvalidOperationException("A failed result must have an error message.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
}

/// <summary>
/// Represents the result of an operation that returns a value and may succeed or fail.
/// </summary>
public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    private Result(bool isSuccess, T? value, string? error) : base(isSuccess, error)
    {
        _value = value;
    }

    public static Result<T> Success(T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value), "Success result value cannot be null.");

        return new Result<T>(true, value, null);
    }

    public new static Result<T> Failure(string error) => new(false, default, error);

    public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>
/// Represents a paginated result set.
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList().AsReadOnly();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    public static PagedResult<T> Empty(int page, int pageSize) =>
        new([], 0, page, pageSize);
}

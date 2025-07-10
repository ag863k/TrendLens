using System.Diagnostics.CodeAnalysis;

namespace TrendLens.Core.Common;

/// <summary>
/// Represents the result of an operation that returns data
/// </summary>
/// <typeparam name="T">The type of data returned</typeparam>
public sealed class Result<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; private init; }

    /// <summary>
    /// Indicates whether the operation failed
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// The data returned by the operation (null if failed)
    /// </summary>
    public T? Data { get; private init; }

    /// <summary>
    /// Primary error message if the operation failed
    /// </summary>
    public string Error { get; private init; } = string.Empty;

    /// <summary>
    /// Collection of all error messages
    /// </summary>
    public IReadOnlyList<string> Errors { get; private init; } = new List<string>().AsReadOnly();

    /// <summary>
    /// Exception that caused the failure (if any)
    /// </summary>
    public Exception? Exception { get; private init; }

    private Result(bool isSuccess, T? data, string error, IEnumerable<string>? errors = null, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        Exception = exception;
    }

    /// <summary>
    /// Creates a successful result with data
    /// </summary>
    public static Result<T> Success(T data) => new(true, data, string.Empty);

    /// <summary>
    /// Creates a failed result with a single error message
    /// </summary>
    public static Result<T> Failure(string error) => new(false, default, error);

    /// <summary>
    /// Creates a failed result with multiple error messages
    /// </summary>
    public static Result<T> Failure(IEnumerable<string> errors) => 
        new(false, default, string.Empty, errors);

    /// <summary>
    /// Creates a failed result with an exception
    /// </summary>
    public static Result<T> Failure(Exception exception, string? message = null) => 
        new(false, default, message ?? exception.Message, null, exception);

    /// <summary>
    /// Transforms the result to a different type if successful
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (IsFailure) 
            return Result<TNew>.Failure(Error);
        
        if (Data is null)
            return Result<TNew>.Failure("Data is null");
        
        try
        {
            return Result<TNew>.Success(mapper(Data));
        }
        catch (Exception ex)
        {
            return Result<TNew>.Failure(ex);
        }
    }

    /// <summary>
    /// Chains another operation if this result is successful
    /// </summary>
    public async Task<Result<TNew>> BindAsync<TNew>(Func<T, Task<Result<TNew>>> binder)
    {
        if (IsFailure)
            return Result<TNew>.Failure(Error);

        if (Data is null)
            return Result<TNew>.Failure("Data is null");

        try
        {
            return await binder(Data).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Result<TNew>.Failure(ex);
        }
    }

    /// <summary>
    /// Converts to a non-generic Result
    /// </summary>
    public Result ToResult() => IsSuccess ? Result.Success() : Result.Failure(Error);

    /// <summary>
    /// Implicit conversion to boolean (true if successful)
    /// </summary>
    public static implicit operator bool(Result<T> result) => result.IsSuccess;
}

/// <summary>
/// Represents the result of an operation without return data
/// </summary>
public sealed class Result
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; private init; }

    /// <summary>
    /// Indicates whether the operation failed
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Primary error message if the operation failed
    /// </summary>
    public string Error { get; private init; } = string.Empty;

    /// <summary>
    /// Collection of all error messages
    /// </summary>
    public IReadOnlyList<string> Errors { get; private init; } = new List<string>().AsReadOnly();

    /// <summary>
    /// Exception that caused the failure (if any)
    /// </summary>
    public Exception? Exception { get; private init; }

    private Result(bool isSuccess, string error, IEnumerable<string>? errors = null, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        Exception = exception;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result Success() => new(true, string.Empty);

    /// <summary>
    /// Creates a failed result with a single error message
    /// </summary>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a failed result with multiple error messages
    /// </summary>
    public static Result Failure(IEnumerable<string> errors) => 
        new(false, string.Empty, errors);

    /// <summary>
    /// Creates a failed result with an exception
    /// </summary>
    public static Result Failure(Exception exception, string? message = null) => 
        new(false, message ?? exception.Message, null, exception);

    /// <summary>
    /// Chains another operation if this result is successful
    /// </summary>
    public async Task<Result<T>> BindAsync<T>(Func<Task<Result<T>>> binder)
    {
        if (IsFailure)
            return Result<T>.Failure(Error);

        try
        {
            return await binder().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(ex);
        }
    }

    /// <summary>
    /// Implicit conversion to boolean (true if successful)
    /// </summary>
    public static implicit operator bool(Result result) => result.IsSuccess;
}

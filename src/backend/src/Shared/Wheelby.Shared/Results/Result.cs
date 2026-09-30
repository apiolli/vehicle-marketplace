namespace Wheelby.Shared.Results;

/// <summary>
/// Represents the outcome of an operation. A successful result always carries
/// <see cref="Error.None"/>; a failed result always carries an error other than
/// <see cref="Error.None"/>. Any inconsistent state throws <see cref="InvalidOperationException"/> at construction.
/// </summary>
public class Result
{
    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets the error. It is <see cref="Error.None"/> when <see cref="IsSuccess"/> is true.</summary>
    public Error Error { get; }

    /// <summary>Initializes a new instance of the <see cref="Result"/> class.</summary>
    /// <exception cref="InvalidOperationException">Thrown when success/error combination is inconsistent.</exception>
    protected Result(bool isSuccess, Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("A successful result must carry Error.None.");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("A failed result must carry an error other than Error.None.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>Creates a failed result.</summary>
    public static Result Failure(Error error) => new(false, error);
}

/// <summary>
/// Represents the outcome of an operation that produces a value of type <typeparamref name="T"/>.
/// Reading <see cref="Value"/> on a failed result throws <see cref="InvalidOperationException"/>.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>
    /// Gets the success value. Throws <see cref="InvalidOperationException"/> when the result is a failure.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is a failure.</exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot read Value of a failed result.");

    private Result(bool isSuccess, T? value, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>Creates a successful result with the given value.</summary>
    public static Result<T> Success(T value) => new(true, value, Error.None);

    /// <summary>Creates a failed result.</summary>
    public static new Result<T> Failure(Error error) => new(false, default, error);
}

namespace Wheelby.Shared.Results;

/// <summary>Classifies an application or domain error without leaking internal details.</summary>
public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden
}

/// <summary>
/// Immutable error value carried by a failed <see cref="Result"/>.
/// </summary>
/// <remarks>
/// Error code convention: <c>&lt;module&gt;.&lt;snake_case&gt;</c>,
/// for example <c>access_control.duplicate_email</c>.
/// Codes are stable, human-readable identifiers safe to expose to API consumers.
/// Messages are user-facing; never put stack traces, SQL, or other internal details here (RD-08).
/// </remarks>
public sealed record Error(string Code, string Message, ErrorType Type)
{
    /// <summary>Sentinel value meaning "no error". A successful result always carries this instance value.</summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    /// <summary>Creates a generic failure error.</summary>
    public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);

    /// <summary>Creates a validation error.</summary>
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);

    /// <summary>Creates a not-found error.</summary>
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    /// <summary>Creates a conflict error.</summary>
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    /// <summary>Creates an unauthorized error.</summary>
    public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);

    /// <summary>Creates a forbidden error.</summary>
    public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);
}

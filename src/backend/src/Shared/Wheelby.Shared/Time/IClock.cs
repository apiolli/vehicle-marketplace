namespace Wheelby.Shared.Time;

/// <summary>
/// Single source of date and time for the application (RD-11).
/// Abstractions over the system clock so business logic and tests never call
/// <see cref="DateTime.UtcNow"/> directly and every timestamp stays in UTC.
/// </summary>
public interface IClock
{
    /// <summary>Gets the current UTC date and time. Its <see cref="DateTime.Kind"/> is always <see cref="DateTimeKind.Utc"/>.</summary>
    DateTime UtcNow { get; }
}

/// <summary>Default <see cref="IClock"/> implementation backed by the system clock.</summary>
internal sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}

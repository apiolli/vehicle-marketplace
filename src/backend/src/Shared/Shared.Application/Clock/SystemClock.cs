using System;
using Shared.Application.Abstractions;

namespace Shared.Application.Clock;

internal sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}

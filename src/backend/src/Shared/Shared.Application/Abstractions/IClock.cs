using System;

namespace Shared.Application.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}

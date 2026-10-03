using MediatR;
using Shared.Application.Abstractions;

namespace AccessControl.Application.Ping;

public sealed record PingQuery : IQuery<string>;

internal sealed class PingQueryHandler : IRequestHandler<PingQuery, string>
{
    public Task<string> Handle(PingQuery request, CancellationToken cancellationToken)
        => Task.FromResult("pong");
}
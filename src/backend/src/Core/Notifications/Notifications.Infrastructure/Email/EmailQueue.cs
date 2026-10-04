using MediatR;
using Notifications.Application.Features.EnqueueEmail;
using Notifications.Contracts;

namespace Notifications.Infrastructure.Email;

internal sealed class EmailQueue(ISender sender) : IEmailQueue
{
    public Task EnqueueAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        => sender.Send(new EnqueueEmailCommand(to, subject, body), cancellationToken);
}
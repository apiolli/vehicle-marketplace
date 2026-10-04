using MediatR;
using Notifications.Application.Abstractions;
using Notifications.Domain.Emails;
using Shared.Application.Abstractions;

namespace Notifications.Application.Features.EnqueueEmail;

internal sealed class EnqueueEmailCommandHandler(
    IQueuedEmailRepository repository,
    IClock clock) : IRequestHandler<EnqueueEmailCommand, Unit>
{
    public async Task<Unit> Handle(EnqueueEmailCommand request, CancellationToken cancellationToken)
    {
        /* Si el destinatario, asunto o cuerpo son inválidos, el dominio lanza
           InvalidEmailDataException (400) y nada se guarda. */
        var to = EmailAddress.Create(request.To);
        var email = QueuedEmail.Create(to, request.Subject, request.Body, clock.UtcNow);

        await repository.AddAsync(email, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
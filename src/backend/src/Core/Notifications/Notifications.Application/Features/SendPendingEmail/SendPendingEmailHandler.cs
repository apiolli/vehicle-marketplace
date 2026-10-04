using MediatR;
using Notifications.Application.Abstractions;
using Shared.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Notifications.Application.Features.SendPendingEmails;



internal sealed class SendPendingEmailsCommandHandler(
    IQueuedEmailRepository repository,
    IEmailSender sender,
    IClock clock,
    ILogger<SendPendingEmailsCommandHandler> logger)
    : IRequestHandler<SendPendingEmailsCommand, SendPendingEmailsResult>
{
    private const int BatchSize = 50;

    public async Task<SendPendingEmailsResult> Handle(
        SendPendingEmailsCommand request, CancellationToken cancellationToken)
    {
        var pending = await repository.GetPendingAsync(BatchSize, cancellationToken);

        var sent = 0;
        var failed = 0;

        foreach (var email in pending)
        {
            try
            {
                await sender.SendAsync(email.To.Value, email.Subject, email.Body, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Sigue Pending y se reintenta en la próxima ejecución.
                // Se registra el id y el motivo, nunca el cuerpo ni el enlace.
                failed++;
                logger.LogWarning("Email {EmailId} could not be sent: {Reason}",
                    email.Id, ex.GetType().Name);
                continue;
            }

            // Se guarda justo después de cada envío exitoso: si el proceso
            // se cae a mitad del lote, los ya enviados no se reenvían.
            email.MarkAsSent(clock.UtcNow);
            await repository.SaveChangesAsync(cancellationToken);
            sent++;
        }

        return new SendPendingEmailsResult(sent, failed);
    }
}
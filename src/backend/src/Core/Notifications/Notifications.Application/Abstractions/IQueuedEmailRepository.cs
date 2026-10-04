using Notifications.Domain.Emails;

namespace Notifications.Application.Abstractions;

public interface IQueuedEmailRepository
{
    Task AddAsync(QueuedEmail email, CancellationToken cancellationToken);
    // Los más antiguos primero, con un tope para no cargar toda la cola
    Task<IReadOnlyList<QueuedEmail>> GetPendingAsync(int maxCount, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
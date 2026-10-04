using Microsoft.EntityFrameworkCore;
using Notifications.Application.Abstractions;
using Notifications.Domain.Emails;
using Notifications.Domain.Emails.Enums;

namespace Notifications.Infrastructure.Persistence;

internal sealed class QueuedEmailRepository(NotificationsDbContext db) : IQueuedEmailRepository
{
    public async Task AddAsync(QueuedEmail email, CancellationToken cancellationToken)
        => await db.QueuedEmails.AddAsync(email, cancellationToken);

    public async Task<IReadOnlyList<QueuedEmail>> GetPendingAsync(
        int maxCount, CancellationToken cancellationToken)
        => await db.QueuedEmails
            .Where(e => e.Status == EmailStatus.Pending)
            .OrderBy(e => e.CreatedAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => db.SaveChangesAsync(cancellationToken);
}
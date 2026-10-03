using Notifications.Domain.Emails.Enums;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

namespace Notifications.Domain.Emails;

public sealed class QueuedEmail : Entity<Guid>
{
    public string To { get; private set; } = default!;
    public string Subject { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public EmailStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    // Constructor privado para EF Core
    private QueuedEmail() : base(Guid.Empty) { }

    private QueuedEmail(Guid id, string to, string subject, string body, DateTime utcNow)
        : base(id)
    {
        To = to;
        Subject = subject;
        Body = body;
        Status = EmailStatus.Pending;
        CreatedAt = utcNow;
    }

    public static QueuedEmail Create(string to, string subject, string body, DateTime utcNow)
        => new(Guid.NewGuid(), to, subject, body, utcNow);

    public void MarkAsSent(DateTime utcNow)
    {
        if (Status == EmailStatus.Sent)
            throw new ConflictException("El correo ya fue enviado.");

        Status = EmailStatus.Sent;
        SentAt = utcNow;
    }
}
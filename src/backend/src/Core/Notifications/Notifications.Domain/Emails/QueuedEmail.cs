using Notifications.Domain.Emails.Enums;
using Notifications.Domain.Emails.Exceptions;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

namespace Notifications.Domain.Emails;

public sealed class QueuedEmail : Entity<Guid>
{
    public const int SubjectMaxLength = 200;

    public EmailAddress To { get; private set; } = default!;
    public string Subject { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public EmailStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    // Constructor privado para EF Core
    private QueuedEmail() : base(Guid.Empty) { }

    private QueuedEmail(Guid id, EmailAddress to, string subject, string body, DateTime utcNow)
        : base(id)
    {
        To = to;
        Subject = subject;
        Body = body;
        Status = EmailStatus.Pending;
        CreatedAt = utcNow;
    }

    public static QueuedEmail Create(EmailAddress to, string? subject, string? body, DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(to);

        var cleanSubject = subject?.Trim();
        if (string.IsNullOrEmpty(cleanSubject))
            throw new InvalidEmailDataException("El asunto del correo es obligatorio.");
            
        if (cleanSubject.Length > SubjectMaxLength)
            throw new InvalidEmailDataException("El asunto del correo es demasiado largo.");

        if (string.IsNullOrWhiteSpace(body))
            throw new InvalidEmailDataException("El cuerpo del correo es obligatorio.");

        return new QueuedEmail(Guid.NewGuid(), to, cleanSubject, body, utcNow);
    }

    public void MarkAsSent(DateTime utcNow)
    {
        if (Status == EmailStatus.Sent)
            throw new ConflictException("El correo ya fue enviado.");

        Status = EmailStatus.Sent;
        SentAt = utcNow;
    }
}
using System.Net.Mail;
using Notifications.Domain.Emails.Exceptions;

namespace Notifications.Domain.Emails;

public sealed record EmailAddress
{
    public const int MaxLength = 254;
    public string Value { get; }
    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string? value)
    {
        var trimmed = value?.Trim();

        if (string.IsNullOrEmpty(trimmed))
            throw new InvalidEmailDataException("El destinatario del correo es obligatorio.");

        if (trimmed.Length > MaxLength)
            throw new InvalidEmailDataException("El destinatario del correo es demasiado largo.");

        // TryCreate también acepta formas como "Nombre <a@b.com>"; exigimos que sea solo la dirección.
        if (!MailAddress.TryCreate(trimmed, out var parsed) || parsed.Address != trimmed)
            throw new InvalidEmailDataException("El destinatario del correo no tiene un formato válido.");

        return new EmailAddress(trimmed);
    }

    public override string ToString() => Value;
}
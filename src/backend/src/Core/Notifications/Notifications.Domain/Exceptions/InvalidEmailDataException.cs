using SharedKernel.Exceptions;

namespace Notifications.Domain.Emails.Exceptions;

public sealed class InvalidEmailDataException(string message) : AppException(message)
{
    public override int StatusCode => 400;
}
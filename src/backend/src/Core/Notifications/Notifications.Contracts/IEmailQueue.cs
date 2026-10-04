namespace Notifications.Contracts;

public interface IEmailQueue
{
    Task EnqueueAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
using Shared.Application.Abstractions;

namespace Notifications.Application.Features.EnqueueEmail;

public sealed record EnqueueEmailCommand(string To, string Subject, string Body) : ICommand;

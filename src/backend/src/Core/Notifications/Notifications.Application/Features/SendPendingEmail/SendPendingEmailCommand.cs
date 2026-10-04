using Shared.Application.Abstractions;

namespace Notifications.Application.Features.SendPendingEmails;

public sealed record SendPendingEmailsCommand : ICommand<SendPendingEmailsResult>;

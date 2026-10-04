using Notifications.Infrastructure;
using Shared.Application;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSharedApplication();
builder.Services.AddNotifications(builder.Configuration);

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

// Procesa una sola pasada y termina
await host.Services.ApplyNotificationsMigrationsAsync();
var (sent, failed) = await host.Services.SendPendingEmailsAsync();

logger.LogInformation("Pending emails processed. Sent: {Sent}. Failed: {Failed}.", sent, failed);

return failed > 0 ? 1 : 0;
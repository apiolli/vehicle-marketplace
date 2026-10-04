using Host.WebAPI.ErrorHandling;
using Shared.Application;
using Scalar.AspNetCore;
using AccessControl.Infrastructure;
using Notifications.Infrastructure;
using Notifications.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddSharedApplication();  
builder.Services.AddAccessControl();       
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddNotifications(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
await app.Services.ApplyNotificationsMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Wheelby API");
    });                              
}

app.MapPost("/test/enqueue-email", async (EnqueueEmailRequest request, IEmailQueue queue) =>
{
    await queue.EnqueueAsync(request.To, request.Subject, request.Body);
    return Results.Accepted();
});


app.Run();
internal sealed record EnqueueEmailRequest(string To, string Subject, string Body);
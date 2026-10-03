using Host.WebAPI.ErrorHandling;
using Shared.Application;
using Scalar.AspNetCore;
using AccessControl.Infrastructure;
using MediatR;
using AccessControl.Application.Ping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddSharedApplication();   // IClock + ValidationBehavior (una sola vez)
builder.Services.AddAccessControl();       // handlers y validadores del módulo
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Wheelby API");
    });                              
}

app.MapGet("/test/ping", async (ISender sender)
    => await sender.Send(new PingQuery()));

app.MapPost("/test/echo", async (EchoCommand command, ISender sender)
    => await sender.Send(command));

app.Run();
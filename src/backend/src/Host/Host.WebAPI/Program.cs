using Host.WebAPI.ErrorHandling;
using Scalar.AspNetCore;
using SharedKernel.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                 // /openapi/v1.json
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Wheelby API");
    });                               // /scalar/v1
}

// Endpoints temporales de prueba
app.MapGet("/test/conflict", () =>
{
    throw new ConflictException("prueba");
});

app.MapGet("/test/error", () =>
{
    throw new Exception("secreto interno");
});

app.Run();
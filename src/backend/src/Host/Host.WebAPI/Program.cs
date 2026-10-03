using Host.WebAPI.ErrorHandling;
using Shared.Application;
using Scalar.AspNetCore;
using AccessControl.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddSharedApplication();  
builder.Services.AddAccessControl();       
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


app.Run();
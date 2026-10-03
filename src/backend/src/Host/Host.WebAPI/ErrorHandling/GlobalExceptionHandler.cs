using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SharedKernel.Exceptions;

namespace Host.WebAPI.ErrorHandling;

internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int status;
        string detail;

        if (exception is AppException appException)
        {
            logger.LogWarning("Request rejected ({Status}): {Message}",
                appException.StatusCode, appException.Message);

            status = appException.StatusCode;
            detail = appException.Message;
        }
        else
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);

            status = StatusCodes.Status500InternalServerError;
            detail = "Ocurrió un error interno del servidor.";
        }

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = ReasonPhrases.GetReasonPhrase(status),
                Detail = detail
            }
        });
    }
}
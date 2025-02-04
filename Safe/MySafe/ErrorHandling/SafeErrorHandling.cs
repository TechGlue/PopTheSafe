using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MySafe.ErrorHandling;

/// <summary>
/// Todo: probably want to limit error information sent to the client for safety purposes. For learning this is fine.
/// Exception handler reserved for safe exceptions that are not caught.
/// These exceptions are going to be only invalid safe-pin inputs from client.
/// </summary>
public class SafeErrorHandling : IExceptionHandler
{
    private readonly ILogger<SafeErrorHandling> _logger;

    public SafeErrorHandling(ILogger<SafeErrorHandling> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionMessage = exception.Message;
        var exceptionType = exception.GetType().Name;

        _logger.LogError(
            "Exception Message: {exceptionType} - {exceptionMessage}",
            exceptionType, exceptionMessage);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = $"Server error - {exceptionType}",
            Detail = exceptionMessage
        };

        // append required status code to response
        httpContext.Response.StatusCode = problemDetails.Status.Value;

        // send error response to client
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
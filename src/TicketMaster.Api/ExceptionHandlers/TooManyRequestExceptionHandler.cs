
using Microsoft.Extensions.Logging;

namespace TicketMaster.Domain.Exceptions.ExceptionHandlers;

public class TooManyRequestExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<TooManyRequestExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not TooManyRequestsException tooManyRequestsException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        httpContext.Response.Headers.RetryAfter = tooManyRequestsException.RetryAfter;

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Rate limiting exception",
                Instance = httpContext.Request.Path,
                Detail = exception.Message
            },
            
        };

        logger.LogWarning("Rate limit exceeded for IP: {IpAddress}",
            httpContext.Connection.RemoteIpAddress);

        return await problemDetailsService.TryWriteAsync(context);

    }
}
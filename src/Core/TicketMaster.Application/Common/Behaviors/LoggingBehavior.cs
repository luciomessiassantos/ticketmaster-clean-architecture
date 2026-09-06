using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace TicketMaster.Application.Common.Behaviors;


public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling {RequestName} {@Request}", typeof(TRequest).Name, request
        );

        var timer = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);

            timer.Stop();

            logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds}ms",
                typeof(TRequest).Name,
                timer.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            timer.Stop();

            logger.LogError(
                ex,
                "FAIL {RequestName} in {ElapsedMilliseconds}ms",
                typeof(TRequest).Name,
                timer.ElapsedMilliseconds);

            throw;
        }



        
    }
}
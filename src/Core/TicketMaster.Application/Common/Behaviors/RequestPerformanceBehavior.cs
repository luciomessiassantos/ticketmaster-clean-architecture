using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace TicketMaster.Application.Common.Behaviors;


public class RequestPerformanceBehavior<TRequest, TResponse>(
        ILogger<RequestPerformanceBehavior<TRequest, TResponse>> logger
) :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        TResponse response = await next();
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > TimeSpan.FromSeconds(5).TotalMilliseconds)
        {
            logger.LogWarning("{request} has taken {Time} to run completely", request, stopwatch.ElapsedMilliseconds);
        }
        return response;
    }
}
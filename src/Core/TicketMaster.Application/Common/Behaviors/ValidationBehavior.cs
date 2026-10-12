using FluentValidation;
using MediatR;
using TicketMaster.Application.Common.Models;
using TicketMaster.Application.Features.Events.Dtos;
using TicketMaster.Domain.Shared;

namespace TicketMaster.Application.Common.Behaviors;


public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = results
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();
            
            if (failures.Count > 0) 
                return (TResponse)(object)Result<EventDto>.Failure(
                ValidationError.Create(failures));

        }

        return await next(cancellationToken);
    }
}
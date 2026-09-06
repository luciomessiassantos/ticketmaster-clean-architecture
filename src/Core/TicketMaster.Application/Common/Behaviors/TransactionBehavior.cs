using System.Transactions;
using MediatR;

namespace TicketMaster.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Esse behavior vale apenas para Commands: operações de mutação de dados no banco
        if (typeof(TRequest).Name.EndsWith("Query"))
        {
            return await next(cancellationToken);
        }

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.MaximumTimeout  
        };

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required, 
            transactionOptions, 
            TransactionScopeAsyncFlowOption.Enabled);
            
        TResponse response = await next(cancellationToken);

        transaction.Complete();

        return response;
    }
}
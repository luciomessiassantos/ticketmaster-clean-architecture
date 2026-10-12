using MediatR;
using TicketMaster.Application.Common.Interfaces;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Interfaces;
using TicketMaster.Infrastructure.Data.Repositories;

namespace TicketMaster.Infrastructure.Data;

// public class UnitOfWork(
//     TicketMasterDbContext context,
//     IMediator mediator
// ) : IUnitOfWork
// {
//     private readonly TicketMasterDbContext _context = context;
//     private EventRepository? _eventRepository;
//     private OrderRepository? _orderRepository;


//     public IEventRepository Events
//         => _eventRepository ??= new EventRepository(_context);

//     public IOrderRepository Orders
//         => _orderRepository ??= new OrderRepository(_context);


//     public void Dispose()
//     {
//         context.Dispose();
//     }

//     /// <summary>
//     ///     Equivalente a Commit de todas as Transactions efetuadas em uma operação
//     ///     do banco de dados.
//     ///     Mapeia todos os Aggregate Roots com eventos de domínio pendentes e os
//     ///     executa. 
//     /// </summary>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//     {
//         var changedEntities = _context.ChangeTracker
//             .Entries<AggregateRoot>()
//             .Where(e => e.Entity.DomainEvents.Count > 0)
//             .SelectMany(e => e.Entity.DomainEvents);
        
//         foreach (var changed in changedEntities)
//         {
//             await mediator.Publish(changed, cancellationToken);
//         }

//         return await _context.SaveChangesAsync(cancellationToken);
//     }

//     public Task<bool> SaveChangesWithResultAsync(CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
// }



public class UnitOfWork(
    TicketMasterDbContext context,
    IMediator mediator,
    IEventRepository eventRepository,
    IOrderRepository orderRepository) : IUnitOfWork
{
    private readonly TicketMasterDbContext _context = context;
    private readonly IMediator _mediator = mediator;

    public IEventRepository Events { get; } = eventRepository;
    public IOrderRepository Orders { get; } = orderRepository;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Pega todas as classes marcadas como AggregateRoot
        var aggregateRoots = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        // Coleta seus eventos de domínio
        var domainEvents = aggregateRoots
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Limpa os eventos de domínio
        aggregateRoots.ForEach(a => a.ClearDomainEvents());

        // Commita a transação
        var result = await _context.SaveChangesAsync(cancellationToken);

        // Publica cada um deles
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public Task<bool> SaveChangesWithResultAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
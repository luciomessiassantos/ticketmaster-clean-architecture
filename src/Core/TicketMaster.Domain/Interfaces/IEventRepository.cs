using Ardalis.Specification;
using TicketMaster.Domain.Entities;
using TicketMaster.Domain.Entities.EventAggregate;

namespace TicketMaster.Domain.Interfaces;


public interface IEventRepository
{

    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Event>> ListAsync(ISpecification<Event>? spec = null, CancellationToken cancellationToken = default);

    Task<int> CountAsync(ISpecification<Event>? spec = null, CancellationToken cancellationToken = default);

    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
    
    void Delete(Event @event);
}
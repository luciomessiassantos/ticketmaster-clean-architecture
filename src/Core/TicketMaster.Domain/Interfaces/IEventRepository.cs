using Ardalis.Specification;
using TicketMaster.Domain.Entities;
using TicketMaster.Domain.Entities.EventAggregate;

namespace TicketMaster.Domain.Interfaces;


public interface IEventRepository : IRepository<Event>
{

    new Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    new Task<IReadOnlyList<Event>> ListAsync(ISpecification<Event>? spec = null, CancellationToken cancellationToken = default);
    new Task<int> CountAsync(ISpecification<Event>? spec = null, CancellationToken cancellationToken = default);

    new Task AddAsync(Event @event, CancellationToken cancellationToken = default);

    new void Add(Event @event);
    
    new void Delete(Event @event);
}
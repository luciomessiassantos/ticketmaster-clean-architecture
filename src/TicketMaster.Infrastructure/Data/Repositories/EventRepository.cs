using Ardalis.Specification;
using TicketMaster.Domain.Entities.EventAggregate;
using TicketMaster.Domain.Interfaces;

namespace TicketMaster.Infrastructure.Data.Repositories;

public class EventRepository(
    TicketMasterDbContext context
) : IEventRepository
{
    public Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Add(Event @event)
    {
        context.Events.Add(@event);
    }

    public Task<int> CountAsync(ISpecification<Event>? spec = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Delete(Event @event)
    {
        throw new NotImplementedException();
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Event>> ListAsync(ISpecification<Event>? spec = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}
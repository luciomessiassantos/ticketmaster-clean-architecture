using Ardalis.Specification;
using TicketMaster.Domain.Entities;
using TicketMaster.Domain.Entities.EventAggregate;

namespace TicketMaster.Domain.Specifications;

public sealed class EventWithSessionsSpecification : Specification<Event>
{
    public EventWithSessionsSpecification(Guid eventId)
    {
        Query
            .Where(e => e.Id == eventId)
            .Include(e => e.Sessions); 
    }
    
}
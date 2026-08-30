using TicketMaster.Domain.Common;

namespace TicketMaster.Domain.Events;

public record OrderTicketsIssuedDomainEvent(
    Guid OrderId
) : IDomainEvent;
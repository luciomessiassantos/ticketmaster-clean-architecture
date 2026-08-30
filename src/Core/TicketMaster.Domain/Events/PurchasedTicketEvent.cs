using TicketMaster.Domain.Common;

namespace TicketMaster.Domain.Events;


public record PurchasedTicketEvent(
    Guid EventId,
    Guid TicketId,
    Guid UserId,
    int Quantity
) : IDomainEvent;
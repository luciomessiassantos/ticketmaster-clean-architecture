using TicketMaster.Domain.Common;

namespace TicketMaster.Domain.Events;


public record PurchaseTicketLotEvent(
    Guid EventId,
    Guid SessionId,
    Guid TicketLotId,
    Guid CustomerId,
    int Quantity,
    DateTimeOffset Timestamp
) : IDomainEvent;
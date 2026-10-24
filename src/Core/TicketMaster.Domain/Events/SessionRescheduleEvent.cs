using TicketMaster.Domain.Common;

namespace TicketMaster.Domain.Events;


public record SessionRescheduleEvent(
    Guid SessionId,
    DateTimeOffset NewEndDate
) : IDomainEvent;
namespace TicketMaster.Domain.Shared;



public enum ErrorType
{
    ValidationError,
    NotFound,
    DomainError,
    HttpError,
    InternalError
}

// public record Error(
//     string Id,
//     ErrorType Type,
//     string Description,
//     object? Details = null
// );

/// <summary>
///     Erros documentados:
///     1. UnavailableEvent
///     2. TicketOutOfStock
///     3. TicketNotFound
///     4. DuplicatedTicket
///     5. UsedTicket
///     6. InvalidTicket
/// </summary>





public class Error(
    string id,
    ErrorType type,
    string description,
    object? details = null
    )
{

    public string Id { get; set; } = id;
    public ErrorType Type { get; set; } = type;
    public string Description { get; set; } = description;
    public object? Details { get; set; } = details;

    
    public static Error TicketOutOfStock
        => new (nameof(TicketOutOfStock), ErrorType.DomainError, $"Ticket out of stock");
    public static Error TicketUnavailable
        => new (nameof(TicketUnavailable), ErrorType.DomainError, $"Ticket is unavailable");
    public static Error TicketAvailableSoon
        => new (nameof(TicketAvailableSoon), ErrorType.DomainError, $"Ticket will be available soon");
    public static Error TicketNotFound
        => new (nameof(TicketNotFound), ErrorType.DomainError, $"Ticket for the event was not found");
    public static Error DuplicatedTicket 
        => new (nameof(DuplicatedTicket), ErrorType.DomainError, $"Ticket Type or Lot already exists for the event");
    public static Error UsedTicket 
        => new (nameof(UsedTicket), ErrorType.DomainError, $"Ticket was already used");
    public static Error InvalidTicket
        => new (nameof(InvalidTicket), ErrorType.DomainError, $"Ticket is invalid");
    public static Error TicketOrderNotFound
        => new (nameof(TicketOrderNotFound), ErrorType.DomainError, $"Ticket order was not found");
    
}


public class EventError(
    string id, 
    ErrorType type, 
    string description, 
    object? details = null
    ) : Error(
            id, 
            type, 
            description, 
            details
        )
{
    public static Error UnavailableEvent => new (nameof(UnavailableEvent), ErrorType.DomainError, $"The Event was Cancelled or is Unavailable at the moment");
    public static Error UnavailableToEditEvent => new (nameof(UnavailableToEditEvent), ErrorType.DomainError, $"The Event is not available for editing it's details");
    public static Error UnableToRemoveSession => new (nameof(UnableToRemoveSession), ErrorType.InternalError, $"Unable to remove session from event");
}


public class SessionError(
    string id, 
    ErrorType type, 
    string description, 
    object? details = null
    ) : Error(
            id, 
            type, 
            description, 
            details
        )
{
    public static Error SessionNotFound => new (nameof(SessionNotFound), ErrorType.NotFound, $"The Session was not found");
    public static Error SessionSoon => new (nameof(SessionSoon), ErrorType.DomainError, $"This Session will be available soon");
    public static Error SessionOutOfStock => new (nameof(SessionOutOfStock), ErrorType.DomainError, $"This Session is out of stock");
    public static Error SessionUnavailable => new (nameof(SessionUnavailable), ErrorType.DomainError, $"This Session is unavailable");
    
}


public class TicketLotError(
    string id, 
    ErrorType type, 
    string description, 
    object? details = null
    ) : Error(
            id, 
            type, 
            description, 
            details
        )
{
    public static Error TicketLotNotFound => new (nameof(TicketLotNotFound), ErrorType.NotFound, $"Ticket Lot not found");
    public static Error TicketLotOutOfStock => new (nameof(TicketLotOutOfStock), ErrorType.NotFound, $"Ticket Lot out of stock at the moment");
    public static Error TicketLotUnavailable => new (nameof(TicketLotUnavailable), ErrorType.NotFound, $"Ticket Lot is unavailable at the moment");
    
}


public class OrderError(
    string id, 
    ErrorType type, 
    string description, 
    object? details = null
    ) : Error(
            id, 
            type, 
            description, 
            details
        )
{
    public static Error TicketsAlreadyIssued => new (nameof(TicketsAlreadyIssued), ErrorType.DomainError, $"Tickets from order were already issued");
    public static Error IssuedTicketNotFound => new (nameof(IssuedTicketNotFound), ErrorType.NotFound, $"The issued ticket was not found");

}


public class IssueTicketError(
    string id, 
    ErrorType type, 
    string description, 
    object? details = null
    ) : Error(
            id, 
            type, 
            description, 
            details
        )
{
    public static Error IssuedTicketAlreadyUsed => new (nameof(IssuedTicketAlreadyUsed), ErrorType.DomainError, $"The issued ticket was already used");
}
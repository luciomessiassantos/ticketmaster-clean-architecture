namespace TicketMaster.Domain.Common;

public enum ErrorType
{
    ValidationError,
    NotFound,
    DomainError,
    HttpError,
    InternalError
}

/// <summary>
/// Classe base para erros. 
/// Os erros genéricos do sistema (como erros de validação ou nulos) ficam aqui.
/// Erros de domínio específicos de "Tickets" foram movidos para os seus respectivos Aggregates.
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

    public static Error None => new(string.Empty, ErrorType.InternalError, string.Empty);
    
    public static Error NullValue(string? message = null)
        => new(nameof(NullValue), ErrorType.DomainError, message ?? "Value cannot be null");

    public static Error ValidationError(string? message, object? details = null)
        => new(nameof(ValidationError), ErrorType.ValidationError, message ?? "A validation error occurred", details);
}

#region Event Aggregate
// Engloba Event, Session e TicketLot, pois pertencem ao mesmo limite de consistência transacional.

public class EventError(string id, ErrorType type, string description, object? details = null) 
    : Error(id, type, description, details)
{
    public static Error EventNotFound => new(nameof(EventNotFound), ErrorType.NotFound, "The Event was not found");
    public static Error UnavailableEvent => new(nameof(UnavailableEvent), ErrorType.DomainError, "The Event was cancelled or is unavailable at the moment");
    public static Error UnavailableToEditEvent => new(nameof(UnavailableToEditEvent), ErrorType.DomainError, "The Event is not available for editing its details");
    public static Error UnableToRemoveSession => new(nameof(UnableToRemoveSession), ErrorType.InternalError, "Unable to remove session from event");
}

public class SessionError(string id, ErrorType type, string description, object? details = null) 
    : Error(id, type, description, details)
{
    public static Error SessionNotFound => new(nameof(SessionNotFound), ErrorType.NotFound, "The Session was not found");
    public static Error SessionSoon => new(nameof(SessionSoon), ErrorType.DomainError, "This Session will be available soon");
    public static Error SessionOutOfStock => new(nameof(SessionOutOfStock), ErrorType.DomainError, "This Session is out of stock");
    public static Error SessionUnavailable => new(nameof(SessionUnavailable), ErrorType.DomainError, "This Session is unavailable");
    
    // Refinamentos adicionados com base nas regras de negócio discutidas:
    public static Error InvalidDates => new(nameof(InvalidDates), ErrorType.DomainError, "Session start date must be before end date");
    public static Error InvalidSessionDate => new(nameof(InvalidSessionDate), ErrorType.DomainError, "Invalid Session date");
    public static Error SessionTimeConflict => new(nameof(SessionTimeConflict), ErrorType.DomainError, "Session time conflicts with another session in this event");
    public static Error CannotRemoveSessionWithTickets => new(nameof(CannotRemoveSessionWithTickets), ErrorType.DomainError, "Cannot remove a session that already has issued tickets or active lots");
}

public class TicketLotError(string id, ErrorType type, string description, object? details = null) 
    : Error(id, type, description, details)
{
    public static Error TicketLotNotFound => new(nameof(TicketLotNotFound), ErrorType.NotFound, "Ticket Lot not found");
    public static Error TicketLotOutOfStock => new(nameof(TicketLotOutOfStock), ErrorType.DomainError, "Ticket Lot out of stock at the moment");
    public static Error TicketLotUnavailable => new(nameof(TicketLotUnavailable), ErrorType.DomainError, "Ticket Lot is unavailable at the moment");
    public static Error InvalidTicketLotPrice => new(nameof(InvalidTicketLotPrice), ErrorType.DomainError, "Invalid Ticket Lot price");
    public static Error InvalidTicketLotNumber => new(nameof(InvalidTicketLotNumber), ErrorType.DomainError, "Invalid Ticket Lot number");
    public static Error InvalidCapacity => new(nameof(InvalidCapacity), ErrorType.DomainError, "Ticket Lot capacity must be greater than zero");
    public static Error DuplicatedTicket => new(nameof(DuplicatedTicket), ErrorType.DomainError, "Ticket Type or Lot already exists for this session");
}
#endregion

#region Order Aggregate
// Pertence ao fluxo transacional de compra do cliente.

public class OrderError(string id, ErrorType type, string description, object? details = null) 
    : Error(id, type, description, details)
{
    public static Error OrderNotFound => new(nameof(OrderNotFound), ErrorType.NotFound, "Ticket order was not found");
    public static Error TicketsAlreadyIssued => new(nameof(TicketsAlreadyIssued), ErrorType.DomainError, "Tickets from this order were already issued");
    public static Error OrderNotPaid => new(nameof(OrderNotPaid), ErrorType.DomainError, "Cannot issue tickets for an unpaid order");
    public static Error OrderExpired => new(nameof(OrderExpired), ErrorType.DomainError, "The ticket order has expired");
}
#endregion

#region IssuedTicket Aggregate
// Representa o ingresso físico/digital final gerado.

public class IssuedTicketError(string id, ErrorType type, string description, object? details = null) 
    : Error(id, type, description, details)
{
    public static Error IssuedTicketNotFound => new(nameof(IssuedTicketNotFound), ErrorType.NotFound, "The issued ticket was not found");
    public static Error IssuedTicketAlreadyUsed => new(nameof(IssuedTicketAlreadyUsed), ErrorType.DomainError, "The issued ticket was already used");
    public static Error InvalidTicket => new(nameof(InvalidTicket), ErrorType.DomainError, "The issued ticket is invalid or revoked");
}
#endregion

#region Customer Aggregate
// Referente ao comprador/usuário.

public class CustomerError(string id, ErrorType type, string description, object? details = null) 
    : Error(id, type, description, details)
{
    public static Error CustomerNotFound => new(nameof(CustomerNotFound), ErrorType.NotFound, "The customer was not found");
}
#endregion
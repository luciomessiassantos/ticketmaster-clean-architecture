using System.Text.Json.Serialization;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Events;
using TicketMaster.Domain.ValueObjects;
using TicketMasterAPI.src.Domain.Enums;


namespace TicketMaster.Domain.Entities.EventAggregate;

public class Event : AggregateRoot
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public string? Organizer { get; private set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventState EventState { get; private set; }

    private readonly List<Session> _sessions = [];

    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();

    private Event() { }

    public Event(string name, string description, string organizer)
    {
        Name = name;
        Description = description;
        Organizer = organizer;
        EventState = EventState.Draft;
    }

    public void Publish() => EventState = EventState.Available;
    public void Cancel() => EventState = EventState.Cancelled;
    public void Unavailable() => EventState = EventState.Unavailable;

    public Result UpdateDetails(string? name, string? description, string? organizer)
    {
        if (EventState != EventState.Available && EventState != EventState.Draft)
            return Result.Failure(EventError.UnavailableEvent);

        if (!string.IsNullOrEmpty(name)) Name = name;
        if (!string.IsNullOrEmpty(description)) Description = description;
        if (!string.IsNullOrEmpty(organizer)) Organizer = organizer;
        
        return Result.Success();
    }

    public Result RegisterSession(Address address, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (EventState != EventState.Available && EventState != EventState.Draft)
            return Result.Failure(EventError.UnavailableEvent);

        var session = new Session(Id, address, startDate, endDate);
        _sessions.Add(session);

        return Result.Success();
    }

    public Result RemoveSession(Guid sessionId)
    {
        var session = Sessions.FirstOrDefault(t => t.Id == sessionId);

        if (session == null) 
            return Result.Failure(SessionError.SessionNotFound);

        var result = _sessions.Remove(session);
        return result ? Result.Success() : Result.Failure(EventError.UnableToRemoveSession);
    }

    public Result UpdateSession(
        Guid sessionId, 
        DateTimeOffset? startDate = null, 
        DateTimeOffset? endDate = null, 
        Address? newAddress = null
    )
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        
        if (session == null) 
            return Result.Failure(SessionError.SessionNotFound);

        if (EventState != EventState.Available && EventState != EventState.Draft)
            return Result.Failure(EventError.UnavailableEvent);
            
        var updateResult = session.UpdateDetails(startDate, endDate, newAddress);

        if (!updateResult.IsSuccess)
            return Result.Failure(updateResult.Error!);

        foreach (var domainEvent in updateResult.Value!)
        {
            AddDomainEvent(domainEvent);
        }

        return Result.Success();
    }

    public Result AddTicketLotToSession(Guid sessionId, int lot, decimal price, long initialQuantity, TicketType type)
    {
        if (EventState != EventState.Available && EventState != EventState.Draft)
            return Result.Failure(EventError.UnavailableEvent);
        
        var session = Sessions.FirstOrDefault(t => t.Id == sessionId);

        if (session == null) 
            return Result.Failure(SessionError.SessionNotFound);

        return session.AddTicketLot(lot, price, initialQuantity, type);
    }

    public Result PurchaseTicket(Guid customerId, Guid sessionId, Guid ticketLotId, int quantity)
    {
        if (EventState != EventState.Available)
            return Result.Failure(EventError.UnavailableEvent);
        
        var session = Sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session == null)
            return Result.Failure(SessionError.SessionNotFound);
        
        var result = session.PurchaseTicketLot(ticketLotId, quantity);

        if (result.IsSuccess)
            AddDomainEvent(new PurchaseTicketLotEvent(Id, sessionId, ticketLotId, customerId, quantity, DateTimeOffset.UtcNow));

        return result;
    }
}
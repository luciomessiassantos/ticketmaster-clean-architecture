using System.Text.Json.Serialization;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Events;
using TicketMaster.Domain.Shared;
using TicketMasterAPI.src.Domain.Enums;
using TicketMasterAPI.src.Domain.ValueObjects;


namespace TicketMaster.Domain.Entities.EventAggregate;

public class Event : AuditableEntity<Guid>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Organizer { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventState EventState { get; set; }

    public List<Session> Sessions { get; private set; } = [];

    public IReadOnlyCollection<Session> SessionsReadOnly => Sessions.AsReadOnly();

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
        if (EventState != EventState.Available || EventState != EventState.Draft)
            return Result.Failure(EventError.UnavailableEvent);

        if (!string.IsNullOrEmpty(name))
            Name = name;
        if (!string.IsNullOrEmpty(description))
            Description = description;
        
        if (!string.IsNullOrEmpty(organizer))
            Description = organizer;
        
        return Result.Success();
    }


    public Result RegisterSession(
            Address? address,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate
        )
    {
        if (EventState != EventState.Available || EventState != EventState.Draft)
            return Result.Failure(EventError.UnavailableEvent);

        var session = new Session(Id, address, startDate, endDate);
        Sessions.Add(session);

        return Result.Success();
    }

    public Result RemoveSession(Guid sessionId)
    {
        var session = Sessions.FirstOrDefault(t => t.Id == sessionId);

        if (session == null) 
            return Result.Failure(SessionError.SessionNotFound);

        var result = Sessions.Remove(session);
        return result ? Result.Success() : Result.Failure(EventError.UnableToRemoveSession);
    }

    public Result AddTicketLotToSession(
        Guid sessionId,
        int lot,
        decimal price,
        long initialQuantity,
        TicketType type
    )
    {
        if (EventState != EventState.Available || EventState != EventState.Draft)
            return Result.Failure(EventError.UnavailableEvent);
        
        var session = Sessions.FirstOrDefault(t => t.Id == sessionId);

        if (session == null) 
            return Result.Failure(SessionError.SessionNotFound);
        

        var result = session.AddTicketLot(
            lot,
            price,
            initialQuantity,
            type
        );

        return result;
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
            AddDomainEvent(new PurchasedTicketEvent(Id, ticketLotId, customerId, quantity));

        return result;
    }



}
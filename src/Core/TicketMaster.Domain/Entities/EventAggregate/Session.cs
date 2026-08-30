using System.Text.Json.Serialization;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Enums;
using TicketMaster.Domain.Shared;
using TicketMasterAPI.src.Domain.Enums;
using TicketMasterAPI.src.Domain.ValueObjects;

namespace TicketMaster.Domain.Entities.EventAggregate;

public class Session : AuditableEntity<Guid>
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public Address? Address { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public SessionState SessionState { get; private set; }

    public List<TicketLot> TicketLots { get; private set; } = [];

    public IReadOnlyList<TicketLot> TicketLotsReadonly => TicketLots.AsReadOnly();

    private Session() { }

    internal Session(
        Guid eventId, 
        Address? address,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate
        )
    {
        EventId = eventId;
        Address = address;
        StartDate = startDate;
        EndDate = endDate;
        SessionState = SessionState.Soon;
    }

    public void Publish() => SessionState = SessionState.Available;

    public void Soon() => SessionState = SessionState.Soon;

    public void Unavailable() => SessionState = SessionState.Unavailable;

    public Result<TicketLot> AddTicketLot(
        int lot,
        decimal price,
        long initialQuantity,
        TicketType type
    )
    {
        if (SessionState == SessionState.Soon) 
            return Result<TicketLot>.Failure(SessionError.SessionSoon);
        
        if (SessionState == SessionState.OutOfStock) 
            return Result<TicketLot>.Failure(SessionError.SessionSoon);
            
        if (SessionState == SessionState.Unavailable)
            return Result<TicketLot>.Failure(SessionError.SessionUnavailable);
        
        var ticketLot = new TicketLot(Id, lot, price, initialQuantity, type);

        TicketLots.Add(ticketLot);

        return Result<TicketLot>.Success(ticketLot);
    }

    public Result RemoveTicketLot(Guid ticketLotId)
    {
        if (SessionState == SessionState.Soon) 
            return Result.Failure(SessionError.SessionSoon);
        
        if (SessionState == SessionState.OutOfStock) 
            return Result.Failure(SessionError.SessionSoon);
            
        if (SessionState == SessionState.Unavailable)
            return Result.Failure(SessionError.SessionUnavailable);
        
        var ticketLot = TicketLots.FirstOrDefault(tl => tl.Id == ticketLotId);
        if (ticketLot == null)
            return Result.Failure(TicketLotError.TicketLotNotFound);

        TicketLots.Remove(ticketLot);

        return Result.Success();
    }

    public Result PurchaseTicketLot(Guid ticketLotId, int quantity)
    {
        
        var test = SessionStateTest();
        if (!test.IsSuccess && test.Error is not null)
            return test;

        var ticketLot = TicketLots.FirstOrDefault(tl => tl.Id == ticketLotId);
        if (ticketLot == null) 
            return Result.Failure(TicketLotError.TicketLotNotFound);

        var result = ticketLot.Purchase(quantity);

        return result;
    }

    private Result SessionStateTest()
    {
        if (SessionState == SessionState.Soon) 
            return Result.Failure(SessionError.SessionSoon);
        
        if (SessionState == SessionState.OutOfStock) 
            return Result.Failure(SessionError.SessionSoon);
            
        if (SessionState == SessionState.Unavailable)
            return Result.Failure(SessionError.SessionUnavailable);
        return Result.Success();
    }
}


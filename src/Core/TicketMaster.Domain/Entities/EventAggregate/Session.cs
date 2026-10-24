using TicketMaster.Domain.Common;
using TicketMaster.Domain.Enums;
using TicketMaster.Domain.Events;
using TicketMaster.Domain.ValueObjects;
using TicketMasterAPI.src.Domain.Enums;

namespace TicketMaster.Domain.Entities.EventAggregate;

public class Session : AuditableEntity<Guid>
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public Address Address { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public SessionState SessionState { get; private set; }

    private readonly List<TicketLot> _ticketLots = [];

    public IReadOnlyList<TicketLot> TicketLots => _ticketLots.AsReadOnly();

    private Session() { }

    internal Session(
        Guid eventId, 
        Address address,
        DateTimeOffset startDate,
        DateTimeOffset endDate
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

        _ticketLots.Add(ticketLot);

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

        _ticketLots.Remove(ticketLot);

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

public Result<List<IDomainEvent>> UpdateDetails(
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        Address? newAddress = null
    )
    {
        var domainEvents = new List<IDomainEvent>();

        var test = SessionStateTest();
        if (!test.IsSuccess) return Result<List<IDomainEvent>>.Failure(test.Error!);

        if (startDate.HasValue) 
            StartDate = startDate.Value;

        if (endDate.HasValue && endDate.Value != EndDate)
        {
            EndDate = endDate.Value;
            domainEvents.Add(new SessionRescheduleEvent(Id, EndDate));
        }
        
        if (StartDate > EndDate)
            return Result<List<IDomainEvent>>.Failure(SessionError.InvalidSessionDate);

        if (newAddress != null && newAddress != Address) 
        {
            Address = newAddress;
            // domainEvents.Add(new SessionAddressChangedEvent(Id, Address));
        }
    
        return Result<List<IDomainEvent>>.Success(domainEvents);
    }

    public Result UpdateTicketLotDetails(
        Guid ticketLotId, 
        int? newLot = null,
        decimal? newPrice = null,
        TicketType? newType = null
    )
    {
        var ticketLot = _ticketLots.FirstOrDefault(t => t.Id == ticketLotId);

        if (ticketLot == null) return Result.Failure(TicketLotError.TicketLotNotFound);

        return ticketLot.UpdateDetails(newLot, newPrice, newType);
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


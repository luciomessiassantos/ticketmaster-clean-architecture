using TicketMaster.Domain.Common;
using TicketMaster.Domain.Events;
using TicketMasterAPI.src.Domain.Enums;

namespace TicketMaster.Domain.Entities.EventAggregate;

public class TicketLot : AuditableEntity<Guid>
{
    public Guid SessionId { get; private set; }
    public Session? Session { get; private set; }
    public int Lot { get; private set; }
    public decimal Price { get; private set; }
    public long Remaining { get; private set; }
    public TicketType Type { get; private set; }
    public bool Available { get; private set; }

    private TicketLot() { }

    public TicketLot(
        Guid sessionId,
        int lot,
        decimal price,
        long initialQuantity,
        TicketType type
    )
    {
        SessionId = sessionId;
        Lot = lot;
        Price = price;
        Remaining = initialQuantity;
        Type = type;
        Available = true;
    }

    public void Unavailable() => Available = false;

    public Result Purchase(int quantity)
    {
        if (Remaining <= 0) 
            return Result<List<IDomainEvent>>.Failure(TicketLotError.TicketLotOutOfStock);
        if (!Available)
            return Result<List<IDomainEvent>>.Failure(TicketLotError.TicketLotUnavailable);
        
        Remaining -= quantity;

        if (Remaining == 0) Unavailable();

        return Result.Success();
    }


    public Result UpdateDetails(
        int? newLot = null,
        decimal? newPrice = null,
        TicketType? newType = null
    )
    {
        if (newLot != null)
        {
            if (newLot <= 0)
            {
                return Result.Failure(TicketLotError.InvalidTicketLotNumber);
            }
            Lot = (int) newLot;
        }

        if (newPrice != null)
        {
            if (newPrice <= 0)
            {
                return Result.Failure(TicketLotError.InvalidTicketLotPrice);
            }
            Price = (decimal) newPrice;
        }

        if (newType != null)
        {
            Type = (TicketType) newType;
        }

        return Result.Success();
    }

    

}
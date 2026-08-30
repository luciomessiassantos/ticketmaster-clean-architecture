using TicketMaster.Domain.Common;
using TicketMaster.Domain.Shared;

namespace TicketMaster.Domain.Entities.OrderAggregate;


public class IssuedTicket : AuditableEntity<Guid>
{
    public Order? Order { get; } = null!;
    public Guid OrderId { get; }
    public string? ConsumeCodeQR { get; }
    public bool Used { get; private set; }

    private IssuedTicket() { }

    public IssuedTicket(string consumeCodeQR)
    {
        ConsumeCodeQR = consumeCodeQR;
        Used = false;
    }

    public Result UseTicket()
    {
        if (Used) return Result.Failure(OrderError.UsedTicket);

        Used = true;

        return Result.Success();
    }
    
}
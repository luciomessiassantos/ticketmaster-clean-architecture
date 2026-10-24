using TicketMaster.Domain.Common;
using TicketMaster.Domain.Events;
using TicketMaster.Domain.Interfaces;

namespace TicketMaster.Domain.Entities.OrderAggregate;

public class Order : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Guid TicketLotId { get; private set; }
    public int Quantity { get; private set; } 
    public bool Consumed { get; private set; }
    

    public DateTimeOffset ExpirationDate { get; private set; }

    private readonly List<IssuedTicket> _issuedTickets = [];
    public IReadOnlyCollection<IssuedTicket> IssuedTickets => _issuedTickets.AsReadOnly();

    private Order() { }

    public Order(Guid customerId, Guid ticketLotId, int quantity, DateTimeOffset expirationDate)
    {
        CustomerId = customerId;
        TicketLotId = ticketLotId;
        Quantity = quantity;
        ExpirationDate = expirationDate;
    }

    public Result IssueTickets(IQRCodeGenerator qrGenerator, DateTimeOffset currentTime)
    {
        if (currentTime > ExpirationDate)
            return Result.Failure(OrderError.OrderExpired);

        if (IssuedTickets.Count > 0)
            return Result.Failure(OrderError.TicketsAlreadyIssued);

        for (int i = 0; i < Quantity; i++)
        {   
            string uniqueCode = qrGenerator.GenerateUniqueCode(Id, TicketLotId, i);
            _issuedTickets.Add(new IssuedTicket(uniqueCode));
            AddDomainEvent(new IssueTicketEvent(CustomerId, Id, uniqueCode, DateTimeOffset.UtcNow));
        }

        return Result.Success();
    }

    public Result UseIssueTicket(Guid issuedTicketId, DateTimeOffset currentTime)
    {
        if (currentTime > ExpirationDate)
            return Result.Failure(OrderError.OrderExpired);

        var issued = IssuedTickets.FirstOrDefault(i => i.Id == issuedTicketId);

        if (issued == null)
            return Result.Failure(IssuedTicketError.IssuedTicketNotFound);
        
        return issued.UseTicket();
    }

    public Result UpdateExpirationDate(DateTimeOffset newEndDate)
    {
        if (newEndDate < DateTimeOffset.Now) return Result.Failure(OrderError.ValidationError("Invalid Date"));
        ExpirationDate = newEndDate;

        return Result.Success();
    }
}
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Entities.EventAggregate;
using TicketMaster.Domain.Events;
using TicketMaster.Domain.Interfaces;
using TicketMaster.Domain.Shared;


namespace TicketMaster.Domain.Entities.OrderAggregate;


public class Order : AuditableEntity<Guid>
{
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; } = null!;
    public Guid TicketLotId { get; }
    public TicketLot? TicketLot { get; }
    public int Quantity { get; private set; } 
    public bool Consumed { get; private set; }

    public List<IssuedTicket> IssuedTickets { get; private set; } = [];
    public IReadOnlyCollection<IssuedTicket> IssuedTicketsReadonly => IssuedTickets.AsReadOnly();

    private Order() { }

    public Order(Guid customerId, Guid ticketLotId, int quantity)
    {
        CustomerId = customerId;
        TicketLotId = ticketLotId;
        Quantity = quantity;
    }

    public Result IssueTickets(IQRCodeGenerator qrGenerator)
    {
        if (IssuedTickets.Count > 0)
            return Result.Failure(OrderError.TicketsAlreadyIssued);

        for (int i = 0; i < Quantity; i++)
        {   
            // Retorna um endpoint formatado da aplicação
            string uniqueCode = qrGenerator.GenerateUniqueCode(Id, TicketLotId, i);
            
            IssuedTickets.Add(new IssuedTicket(uniqueCode));

            // Adiciona aos eventos a serem disparados
            AddDomainEvent(new OrderTicketsIssuedDomainEvent(Id));
        }

        return Result.Success();
    }

    public Result UseIssueTicket(Guid issuedTicketId)
    {
        var issued = IssuedTickets.FirstOrDefault(i => i.Id == issuedTicketId);

        if (issued == null)
            return Result.Failure(OrderError.IssuedTicketNotFound);
        
        if (issued.Used)
            return Result.Failure(IssueTicketError.IssuedTicketAlreadyUsed);

        return issued.UseTicket();
    }

}
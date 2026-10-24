using TicketMaster.Domain.Common;

namespace TicketMaster.Domain.Entities.OrderAggregate;

public class IssuedTicket : AuditableEntity<Guid>
{
    public string ConsumeCodeQR { get; private set; } = string.Empty;
    public bool Used { get; private set; }

    private IssuedTicket() { }

    // Construtor internal: Apenas o Order pode fabricar ingressos
    internal IssuedTicket(string consumeCodeQR)
    {
        ConsumeCodeQR = consumeCodeQR;
        Used = false;
    }

    // Método internal: Apenas o Order pode validar o uso do ingresso
    internal Result UseTicket()
    {
        if (Used) return Result.Failure(IssuedTicketError.IssuedTicketAlreadyUsed);

        Used = true;
        return Result.Success();
    }
}
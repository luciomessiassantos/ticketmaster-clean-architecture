namespace TicketMaster.Domain.Interfaces;

public interface IQRCodeGenerator
{
    string GenerateUniqueCode(Guid orderId, Guid ticketLotId, int ticketIndex);
}
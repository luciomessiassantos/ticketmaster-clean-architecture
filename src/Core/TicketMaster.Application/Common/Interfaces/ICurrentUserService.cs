using TicketMaster.Domain.Shared;

namespace TicketMaster.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Task<Result<Guid>> GetCurrentUserId();
}
using TicketMaster.Application.Common.Models;
using TicketMaster.Domain.Shared;

namespace TicketMaster.Application.Common.Interfaces;


public interface IExternalTokensStorage
{
    Task StoreTokensAsync(
        Guid userId, 
        string provider, 
        string accessToken, 
        string refreshToken, 
        int expiresInSeconds, 
        CancellationToken cancellationToken = default);

    Task<Result<ExternalTokenDto>> RetrieveTokensAsync(
        Guid userId, 
        string provider, 
        CancellationToken cancellationToken = default);

    Task RevokeTokensAsync(
        Guid userId, 
        string provider, 
        CancellationToken cancellationToken = default);
}

using TicketMaster.Application.Common.Models;

namespace TicketMaster.Application.Common.Interfaces.Google;


public interface IGoogleUserAuthService
{
    Task<string> GetAuthUrlAsync(string clientRedirectUri, CancellationToken cancellationToken);
    Task<ExternalAuthTokenResult> GetTokensFromCodeAsync(string Code, CancellationToken cancellationToken);
}
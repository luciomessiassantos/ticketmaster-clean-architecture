namespace TicketMaster.Application.Common.Models;


public record ExternalAuthTokenResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresInSeconds
);
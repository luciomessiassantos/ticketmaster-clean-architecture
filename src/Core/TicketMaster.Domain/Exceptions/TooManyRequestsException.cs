namespace TicketMasterAPI.src.Shared.Exceptions;

public class TooManyRequestsException(
    string retryAfter
) : Exception("Rate limit exceeded")
{
    public string RetryAfter { get; } = retryAfter;
}
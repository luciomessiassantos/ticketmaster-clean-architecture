using TicketMaster.Domain.Common;

namespace TicketMaster.Domain.Entities;

public class Customer : AggregateRoot 
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public Guid UserAuthId { get; private set; }

    private Customer() { }

    public Customer(Guid userAuthId, string name, string email)
    {
        Name = name;
        Email = email;
        UserAuthId = userAuthId;
    }

    public Result UpdateDetails(string? name, string? email)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(email))
            return Result.Failure(Error.ValidationError("Name or Email must be provided for update"));

        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (!string.IsNullOrWhiteSpace(email))
            Email = email;

        return Result.Success();
    }
}

using TicketMaster.Domain.Common;
using TicketMaster.Domain.Shared;


namespace TicketMaster.Domain.Entities.OrderAggregate;

public class Customer : AuditableEntity<Guid>
{
    public string? Name { get; private set; }
    public string? Email { get; private set; }

    public List<Order> Orders = [];
    public IReadOnlyCollection<Order> OrdersReadonly => Orders.AsReadOnly();

    private Customer() { }

    public Customer(Guid id, string name, string email)
    {
        // UserId association
        Id = id;
        Name = name;
        Email = email;
    }

    public void AddOrder(Order order)
    {
        Orders.Add(order);
    }

    public void RemoveOrder(Order order)
    {
        Orders.Remove(order);
    }

}
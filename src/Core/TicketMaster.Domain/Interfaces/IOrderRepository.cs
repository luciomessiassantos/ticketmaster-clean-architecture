using Ardalis.Specification;
using TicketMaster.Domain.Entities.OrderAggregate;

namespace TicketMaster.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{

    new Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    new Task<IReadOnlyList<Order>> ListAsync(ISpecification<Order>? spec = null, CancellationToken cancellationToken = default);

    new Task<int> CountAsync(ISpecification<Order>? spec = null, CancellationToken cancellationToken = default);

    new Task AddAsync(Order order, CancellationToken cancellationToken = default);

    new void Add(Order order);
    
    new void Delete(Order order);
}
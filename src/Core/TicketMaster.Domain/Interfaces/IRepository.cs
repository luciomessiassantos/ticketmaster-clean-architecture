using Ardalis.Specification;
using TicketMaster.Domain.Common;

namespace TicketMaster.Domain.Interfaces;


public interface IRepository<T> where T : AggregateRoot
{

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default);

    Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default);

    Task AddAsync(T @entity, CancellationToken cancellationToken = default);

    void Add(T @entity);
    
    void Delete(T @entity);

}
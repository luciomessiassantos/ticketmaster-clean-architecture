using System.Linq.Expressions;
using Ardalis.Specification;
using TicketMaster.Domain.Entities.EventAggregate;
using TicketMasterAPI.src.Domain.ValueObjects;

namespace TicketMaster.Domain.Specifications;

public class EventWithSessionWithAddressSpecification : Specification<Event>
{
    public EventWithSessionWithAddressSpecification(Address address)
    {
        Expression<Func<Session, bool>> sessionFilter = s => 
            (string.IsNullOrEmpty(address.Country) || s.Address.Country == address.Country) &&
            (string.IsNullOrEmpty(address.State) || s.Address.State == address.State) &&
            (string.IsNullOrEmpty(address.City) || s.Address.City == address.City) &&
            (string.IsNullOrEmpty(address.UF) || s.Address.UF == address.UF) &&
            (string.IsNullOrEmpty(address.ZipCode) || s.Address.ZipCode == address.ZipCode) &&
            (string.IsNullOrEmpty(address.Street) || s.Address.Street == address.Street) &&
            (string.IsNullOrEmpty(address.Neighborhood) || s.Address.Neighborhood == address.Neighborhood) &&
            (string.IsNullOrEmpty(address.Place) || s.Address.Place == address.Place);

        
        Query
            .Where(e => e.Sessions.AsQueryable().Any(sessionFilter))
            .Include(e => e.Sessions.AsQueryable().Where(sessionFilter));
    }
}
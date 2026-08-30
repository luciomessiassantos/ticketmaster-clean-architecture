using System.Diagnostics.Tracing;
using Ardalis.Specification;
using TicketMaster.Domain.Entities;
using TicketMaster.Domain.Entities.EventAggregate;
using TicketMasterAPI.src.Domain.Enums;

namespace TicketMaster.Domain.Specifications;


public sealed class EventSearchFilter : Specification<Event>
{
    public EventSearchFilter(
        string? Country,
        string? State,
        string? City,
        string? Neighborhood,
        string? Street,
        string? UF,
        string? ZipCode,
        string? Place,
        
        EventState eventState = EventState.Available
    )
    {   

    }
}
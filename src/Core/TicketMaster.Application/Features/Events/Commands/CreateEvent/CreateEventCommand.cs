using MediatR;
using TicketMaster.Application.Features.Events.Dtos;
using TicketMaster.Domain.Shared;

namespace TicketMaster.Application.Features.Events.Commands.CreateEvent;


public record CreateEventCommand(
    string Name,
    string Description,
    string Organizer
) : IRequest<Result<EventDto>>;
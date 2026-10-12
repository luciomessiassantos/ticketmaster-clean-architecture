using FluentValidation;

namespace TicketMaster.Application.Features.Events.Commands.CreateEvent;

/// <summary>
///     Validator do Command
/// </summary>
public class CreateEventValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventValidator()
    {
        RuleFor(e => e.Name)
            .MaximumLength(200)
            .MinimumLength(5)
            .NotEmpty()
            .NotNull();

        RuleFor(e => e.Description)
            .MaximumLength(255)
            .MinimumLength(5)
            .NotEmpty()
            .NotNull();
        
        RuleFor(e => e.Organizer)
            .MaximumLength(120)
            .MinimumLength(5)
            .NotEmpty()
            .NotNull();
    }
}


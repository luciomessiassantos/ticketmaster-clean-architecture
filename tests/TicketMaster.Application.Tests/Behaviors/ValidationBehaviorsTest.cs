
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using TicketMaster.Application.Common.Behaviors;
using TicketMaster.Application.Features.Events.Commands.CreateEvent;
using TicketMaster.Application.Features.Events.Dtos;
using TicketMaster.Domain.Shared;
using TicketMasterAPI.src.Domain.Enums;

namespace TicketMaster.Application.Tests.Behaviors;
/// <summary>
///     Por ter separado e centralizado a responsabilidade de validação no ValidationBehavior
///     é necessário criar um teste separado para cada validação utilizando o mesmo Mock do ValidationBehavior.
/// </summary>
public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<CreateEventCommand>> _validatorMock;
    private readonly Mock<RequestHandlerDelegate<Result<EventDto>>> _nextDelegateMock;
    private readonly ValidationBehavior<CreateEventCommand, Result<EventDto>> _validationBehavior;

    public ValidationBehaviorTests()
    {
        _validatorMock = new Mock<IValidator<CreateEventCommand>>();
        _nextDelegateMock = new Mock<RequestHandlerDelegate<Result<EventDto>>>();

        var validators = new List<IValidator<CreateEventCommand>> { _validatorMock.Object };

        _validationBehavior = new ValidationBehavior<CreateEventCommand, Result<EventDto>>(validators);
    }

    [Fact]
    public async Task Should_Return_Failure_Result_And_Not_Call_Next_When_Command_Is_Invalid()
    {
        // Arrange
        var invalidCommand = new CreateEventCommand(string.Empty, "Desc", "Tester");
        var validationFailures = new List<ValidationFailure> 
        { 
            new("Name", "Name is required") 
        };
        
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateEventCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailures));

        // Act
        var result = await _validationBehavior.Handle(
            invalidCommand, 
            _nextDelegateMock.Object, 
            CancellationToken.None
        );

        // Assert
        Assert.False(result.IsSuccess);
        _nextDelegateMock.Verify(next => next(), Times.Never);
    }
    
    [Fact]
    public async Task Should_Call_Next_When_Command_Is_Valid()
    {
        // Arrange
        var validCommand = new CreateEventCommand("Valid Name", "Desc", "Tester");
        var expectedResult = Result<EventDto>.Success(new EventDto(Guid.NewGuid(), "Valid Name", "Desc", "Tester", EventState.Draft, []));
        
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateEventCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult()); 

        _nextDelegateMock.Setup(next => next()).ReturnsAsync(expectedResult);

        // Act
        var result = await _validationBehavior.Handle(
            validCommand, 
            _nextDelegateMock.Object, 
            CancellationToken.None
        );

        // Assert
        Assert.True(result.IsSuccess);
        _nextDelegateMock.Verify(next => next(), Times.Once);
    }
}
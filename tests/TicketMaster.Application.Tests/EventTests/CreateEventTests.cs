using FluentValidation;
using MediatR;
using Moq;
using TicketMaster.Application.Common.Behaviors;
using TicketMaster.Application.Common.Interfaces;
using TicketMaster.Application.Features.Events.Commands.CreateEvent;
using TicketMaster.Application.Features.Events.Dtos;
using TicketMaster.Domain.Entities.EventAggregate;
using TicketMaster.Domain.Interfaces;
using TicketMaster.Domain.Shared;

namespace TicketMaster.Application.Tests.EventTests;

public class CreateEventTests
{
    private readonly CreateEventHandler createEventHandler;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IEventRepository> eventRepository;


    public CreateEventTests()
    {
        unitOfWorkMock = new Mock<IUnitOfWork>();
        eventRepository = new Mock<IEventRepository>();

        unitOfWorkMock.Setup(u => u.Events).Returns(eventRepository.Object);
        createEventHandler = new CreateEventHandler(unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Should_Create_When_Command_Is_Valid()
    {
        // Arrange
        var createEventCommand = new CreateEventCommand(
                                                    "Event",
                                                    "Generic Event description",
                                                    "Tester"
                                                );
        // Act
        var result = await createEventHandler.Handle(createEventCommand);

        // Assert
        unitOfWorkMock.Verify(r => r.Events.Add(It.IsAny<Event>()), Times.Once);
        unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}



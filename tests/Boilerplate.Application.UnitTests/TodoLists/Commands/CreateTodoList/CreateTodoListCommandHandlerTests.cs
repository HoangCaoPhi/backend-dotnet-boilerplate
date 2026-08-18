using Boilerplate.Application.TodoLists.Commands.CreateTodoList;
using Boilerplate.Domain.TodoLists;
using Boilerplate.SharedKernel;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Boilerplate.Application.UnitTests.TodoLists.Commands.CreateTodoList;

public sealed class CreateTodoListCommandHandlerTests
{
    private readonly ITodoListRepository _todoListRepository = Substitute.For<ITodoListRepository>();
    private readonly IIdGenerator _idGenerator = Substitute.For<IIdGenerator>();

    [Fact]
    public async Task Handle_ValidCommand_AddsTodoListAndReturnsItsId()
    {
        // Arrange
        var generatedId = Guid.CreateVersion7();
        _idGenerator.NewId().Returns(generatedId);

        var handler = new CreateTodoListCommandHandler(
            _todoListRepository,
            _idGenerator);

        var command = new CreateTodoListCommand(
            Guid.CreateVersion7(),
            "Groceries",
            Colour.Blue.Code);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(generatedId);
        await _todoListRepository.Received(1).AddAsync(
            Arg.Is<TodoList>(list => list.Id == generatedId && list.Title == "Groceries"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NullColourCode_DefaultsToGrey()
    {
        // Arrange
        _idGenerator.NewId().Returns(Guid.CreateVersion7());

        var handler = new CreateTodoListCommandHandler(
            _todoListRepository,
            _idGenerator);

        var command = new CreateTodoListCommand(
            Guid.CreateVersion7(),
            "Groceries",
            null);

        // Act
        await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        await _todoListRepository.Received(1).AddAsync(
            Arg.Is<TodoList>(list => list.Colour == Colour.Grey),
            Arg.Any<CancellationToken>());
    }
}

using Boilerplate.Application.TodoLists.Commands.CompleteTodoItem;
using Boilerplate.Domain.TodoLists;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Boilerplate.Application.UnitTests.TodoLists.Commands.CompleteTodoItem;

public sealed class CompleteTodoItemCommandHandlerTests
{
    private readonly ITodoListRepository _todoListRepository = Substitute.For<ITodoListRepository>();

    [Fact]
    public async Task Handle_TodoListNotFound_ReturnsFailure()
    {
        // Arrange
        _todoListRepository.GetByIdWithItemsAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns((TodoList?)null);

        // Act
        var result = await CreateHandler().Handle(
            new CompleteTodoItemCommand(
                Guid.CreateVersion7(),
                Guid.CreateVersion7()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_ItemNotFound_ReturnsFailure()
    {
        // Arrange
        var todoList = TodoList.Create(
            Guid.CreateVersion7(),
            "Groceries");

        _todoListRepository.GetByIdWithItemsAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(todoList);

        // Act
        var result = await CreateHandler().Handle(
            new CompleteTodoItemCommand(
                todoList.Id,
                Guid.CreateVersion7()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_ValidCommand_CompletesItem()
    {
        // Arrange
        var todoList = TodoList.Create(
            Guid.CreateVersion7(),
            "Groceries");
        var item = todoList.AddItem(
            Guid.CreateVersion7(),
            "Buy milk",
            null,
            PriorityLevel.Low,
            null,
            null);

        _todoListRepository.GetByIdWithItemsAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(todoList);

        // Act
        var result = await CreateHandler().Handle(
            new CompleteTodoItemCommand(
                todoList.Id,
                item.Id),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        item.IsDone.ShouldBeTrue();
    }

    private CompleteTodoItemCommandHandler CreateHandler() => new(_todoListRepository);
}

using Boilerplate.Application.Common.InternalClients.UserService;
using Boilerplate.Application.TodoLists.Commands.CreateTodoItem;
using Boilerplate.Domain.TodoLists;
using Boilerplate.SharedKernel;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Boilerplate.Application.UnitTests.TodoLists.Commands.CreateTodoItem;

public sealed class CreateTodoItemCommandHandlerTests
{
    private readonly ITodoListRepository _todoListRepository = Substitute.For<ITodoListRepository>();
    private readonly IUserServiceClient _userServiceClient = Substitute.For<IUserServiceClient>();
    private readonly IIdGenerator _idGenerator = Substitute.For<IIdGenerator>();

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
            CreateCommand(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_AssigneeDoesNotExist_ReturnsFailure()
    {
        // Arrange
        var todoList = TodoList.Create(
            Guid.CreateVersion7(),
            "Groceries");
        var assigneeUserId = Guid.CreateVersion7();

        _todoListRepository.GetByIdWithItemsAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(todoList);
        _userServiceClient.UserExistsAsync(
                assigneeUserId,
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await CreateHandler().Handle(
            CreateCommand(assigneeUserId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_NoAssignee_AddsItemWithoutCheckingUserService()
    {
        // Arrange
        var todoList = TodoList.Create(
            Guid.CreateVersion7(),
            "Groceries");
        var itemId = Guid.CreateVersion7();

        _todoListRepository.GetByIdWithItemsAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(todoList);
        _idGenerator.NewId().Returns(itemId);

        // Act
        var result = await CreateHandler().Handle(
            CreateCommand(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(itemId);
        todoList.Items.ShouldHaveSingleItem();
        await _userServiceClient.DidNotReceive().UserExistsAsync(
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>());
    }

    private CreateTodoItemCommandHandler CreateHandler()
        => new(
            _todoListRepository,
            _userServiceClient,
            _idGenerator);

    private static CreateTodoItemCommand CreateCommand(Guid? assigneeUserId = null)
        => new(
            Guid.CreateVersion7(),
            "Buy milk",
            null,
            PriorityLevel.Low,
            null,
            assigneeUserId);
}

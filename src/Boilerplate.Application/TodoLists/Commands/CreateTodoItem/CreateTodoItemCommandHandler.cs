using Boilerplate.Application.Common.InternalClients.UserService;
using Boilerplate.Domain.TodoLists;
using Boilerplate.SharedKernel;

namespace Boilerplate.Application.TodoLists.Commands.CreateTodoItem;

public sealed class CreateTodoItemCommandHandler(
    ITodoListRepository todoListRepository,
    IUserServiceClient userServiceClient,
    IIdGenerator idGenerator) : ICommandHandler<CreateTodoItemCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateTodoItemCommand command,
        CancellationToken cancellationToken)
    {
        var todoList = await todoListRepository.GetByIdWithItemsAsync(
            command.TodoListId,
            cancellationToken);

        if (todoList is null)
        {
            return Result<Guid>.Failure(TodoListErrors.NotFound(command.TodoListId));
        }

        if (command.AssigneeUserId is { } assigneeUserId
            && !await userServiceClient.UserExistsAsync(
                assigneeUserId,
                cancellationToken))
        {
            return Result<Guid>.Failure(TodoListErrors.AssigneeNotFound(assigneeUserId));
        }

        var item = todoList.AddItem(
            idGenerator.NewId(),
            command.Title,
            command.Note,
            command.Priority,
            command.Reminder,
            command.AssigneeUserId);

        return Result<Guid>.Success(item.Id);
    }
}

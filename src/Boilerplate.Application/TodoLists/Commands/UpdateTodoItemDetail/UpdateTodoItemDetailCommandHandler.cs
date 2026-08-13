using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Application.TodoLists.Commands.UpdateTodoItemDetail;

public sealed class UpdateTodoItemDetailCommandHandler(ITodoListRepository todoListRepository)
    : ICommandHandler<UpdateTodoItemDetailCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateTodoItemDetailCommand command,
        CancellationToken cancellationToken)
    {
        var todoList = await todoListRepository.GetByIdWithItemsAsync(
            command.TodoListId,
            cancellationToken);

        if (todoList is null)
        {
            return Result.Failure(TodoListErrors.NotFound(command.TodoListId));
        }

        if (todoList.Items.All(item => item.Id != command.TodoItemId))
        {
            return Result.Failure(TodoListErrors.ItemNotFound(command.TodoItemId));
        }

        todoList.UpdateItemDetail(
            command.TodoItemId,
            command.Title,
            command.Note,
            command.Priority,
            command.Reminder);

        return Result.Success();
    }
}

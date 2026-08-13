using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Application.TodoLists.Commands.CompleteTodoItem;

public sealed class CompleteTodoItemCommandHandler(ITodoListRepository todoListRepository)
    : ICommandHandler<CompleteTodoItemCommand, Result>
{
    public async ValueTask<Result> Handle(
        CompleteTodoItemCommand command,
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

        todoList.CompleteItem(command.TodoItemId);

        return Result.Success();
    }
}

namespace Boilerplate.Application.TodoLists.Commands.CompleteTodoItem;

public sealed record CompleteTodoItemCommand(
    Guid TodoListId,
    Guid TodoItemId) : ICommand<Result>;

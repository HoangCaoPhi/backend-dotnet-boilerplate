using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Application.TodoLists.Commands.UpdateTodoItemDetail;

public sealed record UpdateTodoItemDetailCommand(
    Guid TodoListId,
    Guid TodoItemId,
    string Title,
    string? Note,
    PriorityLevel Priority,
    DateTime? Reminder) : ICommand<Result>;

using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Application.TodoLists.Commands.CreateTodoItem;

public sealed record CreateTodoItemCommand(
    Guid TodoListId,
    string Title,
    string? Note,
    PriorityLevel Priority,
    DateTime? Reminder,
    Guid? AssigneeUserId) : ICommand<Result<Guid>>;

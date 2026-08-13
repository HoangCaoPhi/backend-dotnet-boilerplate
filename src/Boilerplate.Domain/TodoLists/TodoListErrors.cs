using Boilerplate.SharedKernel.Results;

namespace Boilerplate.Domain.TodoLists;

public static class TodoListErrors
{
    public static Error NotFound(Guid id) => new(
        "TodoList.NotFound",
        $"TodoList '{id}' was not found.",
        ErrorType.NotFound);

    public static Error ItemNotFound(Guid itemId) => new(
        "TodoList.ItemNotFound",
        $"Todo item '{itemId}' was not found.",
        ErrorType.NotFound);

    public static Error AssigneeNotFound(Guid userId) => new(
        "TodoList.AssigneeNotFound",
        $"User '{userId}' was not found.",
        ErrorType.NotFound);
}

using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Api.Endpoints.App.TodoLists.CreateTodoItem;

public sealed record CreateTodoItemRequest(
    string Title,
    string? Note,
    PriorityLevel Priority,
    DateTime? Reminder,
    Guid? AssigneeUserId);

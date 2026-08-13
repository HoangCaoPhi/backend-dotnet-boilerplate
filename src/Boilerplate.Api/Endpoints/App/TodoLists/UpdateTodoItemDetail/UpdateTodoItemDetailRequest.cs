using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Api.Endpoints.App.TodoLists.UpdateTodoItemDetail;

public sealed record UpdateTodoItemDetailRequest(
    string Title,
    string? Note,
    PriorityLevel Priority,
    DateTime? Reminder);

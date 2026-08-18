using Boilerplate.Domain.Common;

namespace Boilerplate.Domain.TodoLists.Events;

public sealed record TodoItemCompletedDomainEvent(
    Guid TodoListId,
    Guid TodoItemId,
    Guid? AssigneeUserId) : IDomainEvent;

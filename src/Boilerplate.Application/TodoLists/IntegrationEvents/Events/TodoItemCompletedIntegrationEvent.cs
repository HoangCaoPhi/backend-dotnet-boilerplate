using Boilerplate.Application.Common.EventBus;

namespace Boilerplate.Application.TodoLists.IntegrationEvents.Events;

public sealed record TodoItemCompletedIntegrationEvent(
    Guid TodoListId,
    Guid TodoItemId) : IIntegrationEvent;

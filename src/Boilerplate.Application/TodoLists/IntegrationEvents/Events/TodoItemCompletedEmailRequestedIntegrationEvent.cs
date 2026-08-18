using Boilerplate.Application.Common.IntegrationEvents;

namespace Boilerplate.Application.TodoLists.IntegrationEvents.Events;

public sealed record TodoItemCompletedEmailRequestedIntegrationEvent(
    Guid TodoListId,
    Guid TodoItemId) : IIntegrationEvent;

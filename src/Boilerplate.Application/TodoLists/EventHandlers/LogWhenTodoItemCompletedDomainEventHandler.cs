using Boilerplate.Application.Common.DomainEvents;
using Boilerplate.Domain.TodoLists.Events;
using Microsoft.Extensions.Logging;

namespace Boilerplate.Application.TodoLists.EventHandlers;

public sealed class LogWhenTodoItemCompletedDomainEventHandler(ILogger<LogWhenTodoItemCompletedDomainEventHandler> logger)
    : IDomainEventHandler<TodoItemCompletedDomainEvent>
{
    public ValueTask Handle(
        TodoItemCompletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Todo item {TodoItemId} in list {TodoListId} completed",
            notification.TodoItemId,
            notification.TodoListId);

        return ValueTask.CompletedTask;
    }
}

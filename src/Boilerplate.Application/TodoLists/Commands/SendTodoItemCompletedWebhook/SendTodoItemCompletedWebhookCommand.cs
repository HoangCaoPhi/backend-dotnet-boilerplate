using Boilerplate.Application.Common.Outbox;

namespace Boilerplate.Application.TodoLists.Commands.SendTodoItemCompletedWebhook;

public sealed record SendTodoItemCompletedWebhookCommand(
    Guid TodoListId,
    Guid TodoItemId,
    Guid? AssigneeUserId) : IEventualCommand;

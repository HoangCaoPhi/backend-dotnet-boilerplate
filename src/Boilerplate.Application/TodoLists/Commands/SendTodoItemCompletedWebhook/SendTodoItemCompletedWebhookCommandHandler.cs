using Boilerplate.Application.Common.ExternalClients.Webhook;
using Boilerplate.Application.Common.InternalClients.UserService;
using Mediator;

namespace Boilerplate.Application.TodoLists.Commands.SendTodoItemCompletedWebhook;

public sealed class SendTodoItemCompletedWebhookCommandHandler(
    IWebhookClient webhookClient,
    IUserServiceClient userServiceClient) : ICommandHandler<SendTodoItemCompletedWebhookCommand>
{
    public async ValueTask<Unit> Handle(
        SendTodoItemCompletedWebhookCommand command,
        CancellationToken cancellationToken)
    {
        var assignee = await ResolveAssigneeAsync(
            command.AssigneeUserId,
            cancellationToken);

        await webhookClient.NotifyAsync(
            $"Todo item '{command.TodoItemId}' in list '{command.TodoListId}' completed by {assignee}.",
            cancellationToken);

        return Unit.Value;
    }

    private async Task<string> ResolveAssigneeAsync(
        Guid? assigneeUserId,
        CancellationToken cancellationToken)
    {
        if (assigneeUserId is not { } userId)
        {
            return "nobody";
        }

        return await userServiceClient.GetDisplayNameAsync(
            userId,
            cancellationToken) ?? userId.ToString();
    }
}

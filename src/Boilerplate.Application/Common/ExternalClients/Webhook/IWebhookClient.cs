namespace Boilerplate.Application.Common.ExternalClients.Webhook;

public interface IWebhookClient
{
    Task NotifyAsync(
        string message,
        CancellationToken cancellationToken);
}

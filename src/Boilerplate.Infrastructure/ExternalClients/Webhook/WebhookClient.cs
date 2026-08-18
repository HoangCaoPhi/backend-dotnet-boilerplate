using System.Net.Http.Json;
using Boilerplate.Application.Common.ExternalClients.Webhook;

namespace Boilerplate.Infrastructure.ExternalClients.Webhook;

public sealed class WebhookClient(HttpClient httpClient) : IWebhookClient
{
    public async Task NotifyAsync(
        string message,
        CancellationToken cancellationToken)
    {
        await httpClient.PostAsJsonAsync(
            string.Empty,
            new { text = message },
            cancellationToken);
    }
}

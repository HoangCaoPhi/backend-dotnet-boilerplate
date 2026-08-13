using System.Net.Http.Json;
using Boilerplate.Application.Common.ExternalClients.Slack;

namespace Boilerplate.Infrastructure.ExternalClients.Slack;

public sealed class SlackClient(HttpClient httpClient) : ISlackClient
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

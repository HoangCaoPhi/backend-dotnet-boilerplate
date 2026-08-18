using Boilerplate.Application.Common.ExternalClients.Webhook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Infrastructure.ExternalClients.Webhook;

public static class DependencyInjection
{
    public static IServiceCollection AddWebhookClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IWebhookClient, WebhookClient>(client =>
            client.BaseAddress = new Uri(configuration["Webhook:Url"] ?? "https://webhook.internal/placeholder"));

        return services;
    }
}

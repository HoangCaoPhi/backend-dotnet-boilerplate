using Boilerplate.Application.Common.ExternalClients.Slack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Infrastructure.ExternalClients.Slack;

public static class DependencyInjection
{
    public static IServiceCollection AddSlackClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<ISlackClient, SlackClient>(client =>
            client.BaseAddress = new Uri(configuration["Slack:WebhookUrl"] ?? "https://hooks.slack.com/services/placeholder"));

        return services;
    }
}

using Boilerplate.Application.Common.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Boilerplate.Infrastructure.IntegrationEvents;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegrationEvents(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IIntegrationEventPublisher, RabbitMqIntegrationEventPublisher>();

        services.AddSingleton<IConnection>(_ =>
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = configuration["RabbitMq:Host"] ?? "localhost",
                Port = int.TryParse(configuration["RabbitMq:Port"], out var port) ? port : 5672,
                UserName = configuration["RabbitMq:Username"] ?? "guest",
                Password = configuration["RabbitMq:Password"] ?? "guest",
            };

            return connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        return services;
    }
}

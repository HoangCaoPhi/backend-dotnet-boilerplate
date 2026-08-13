using Boilerplate.Application.Common.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Boilerplate.Infrastructure.Outbox;

public static class DependencyInjection
{
    public static IServiceCollection AddOutbox(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IEventBus, OutboxEventBus>();
        services.AddScoped<IIntegrationEventPublisher, RabbitMqIntegrationEventPublisher>();
        services.AddHostedService<OutboxProcessor>();

        services.AddSingleton<IConnection>(_ =>
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = configuration["RabbitMq:Host"] ?? "localhost",
                UserName = configuration["RabbitMq:Username"] ?? "guest",
                Password = configuration["RabbitMq:Password"] ?? "guest",
            };

            return connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        return services;
    }
}

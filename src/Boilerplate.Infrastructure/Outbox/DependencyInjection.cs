using Boilerplate.Application.Common.Outbox;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Infrastructure.Outbox;

public static class DependencyInjection
{
    public static IServiceCollection AddOutbox(this IServiceCollection services)
    {
        services.AddScoped<IOutbox, Outbox>();
        services.AddHostedService<OutboxProcessor>();

        return services;
    }
}

using Boilerplate.Infrastructure.ExternalClients.Slack;
using Boilerplate.Infrastructure.Idempotency;
using Boilerplate.Infrastructure.InternalClients.UserService;
using Boilerplate.Infrastructure.Outbox;
using Boilerplate.Infrastructure.Persistence;
using Boilerplate.SharedKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        services.AddSingleton<IIdGenerator, GuidV7IdGenerator>();

        services.AddPersistence(configuration);
        services.AddIdempotency();
        services.AddOutbox(configuration);
        services.AddUserServiceClient(configuration);
        services.AddSlackClient(configuration);

        return services;
    }
}

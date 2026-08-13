using Boilerplate.Application.Common.Idempotency;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Infrastructure.Idempotency;

public static class DependencyInjection
{
    public static IServiceCollection AddIdempotency(this IServiceCollection services)
    {
        services.AddScoped<IRequestManager, RequestManager>();

        return services;
    }
}

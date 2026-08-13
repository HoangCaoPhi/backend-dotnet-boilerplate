using Boilerplate.Application.Common.Data;
using Boilerplate.Domain.TodoLists;
using Boilerplate.Infrastructure.Persistence.Interceptors;
using Boilerplate.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' not found.");

        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((
            serviceProvider,
            options) =>
            options
                .UseNpgsql(connectionString)
                .AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>()));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IReadApplicationDbContext, ReadApplicationDbContext>();

        services.AddScoped<ITodoListRepository, TodoListRepository>();

        return services;
    }
}

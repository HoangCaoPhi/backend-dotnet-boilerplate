using Boilerplate.Application.Common.InternalClients.UserService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Infrastructure.InternalClients.UserService;

public static class DependencyInjection
{
    public static IServiceCollection AddUserServiceClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
            client.BaseAddress = new Uri(configuration["UserService:BaseUrl"] ?? "https://user-service.internal"));

        return services;
    }
}

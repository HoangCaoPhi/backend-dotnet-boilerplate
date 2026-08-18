using System.Net.Http.Json;
using Boilerplate.Application.Common.InternalClients.UserService;

namespace Boilerplate.Infrastructure.InternalClients.UserService;

public sealed class UserServiceClient(HttpClient httpClient) : IUserServiceClient
{
    public async Task<bool> UserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"/integration-api/users/{userId}/exists",
            cancellationToken);

        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetDisplayNameAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"/integration-api/users/{userId}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var user = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken);

        return user?.DisplayName;
    }

    private sealed record UserResponse(string DisplayName);
}

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
}

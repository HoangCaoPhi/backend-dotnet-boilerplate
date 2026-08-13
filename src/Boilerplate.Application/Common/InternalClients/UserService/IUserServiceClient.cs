namespace Boilerplate.Application.Common.InternalClients.UserService;

public interface IUserServiceClient
{
    Task<bool> UserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken);
}

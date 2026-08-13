namespace Boilerplate.Application.Common.Idempotency;

public interface IRequestManager
{
    Task<bool> ExistsAsync(
        Guid requestId,
        CancellationToken cancellationToken);

    Task CreateRequestAsync(
        Guid requestId,
        string commandName,
        CancellationToken cancellationToken);
}

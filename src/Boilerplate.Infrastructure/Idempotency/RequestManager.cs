using Boilerplate.Application.Common.Idempotency;
using Boilerplate.Infrastructure.Persistence;
using Boilerplate.SharedKernel;

namespace Boilerplate.Infrastructure.Idempotency;

public sealed class RequestManager(
    ApplicationDbContext context,
    ITimeProvider timeProvider) : IRequestManager
{
    public async Task<bool> ExistsAsync(
        Guid requestId,
        CancellationToken cancellationToken)
        => await context.Set<ProcessedRequest>().FindAsync(
            [requestId],
            cancellationToken) is not null;

    public async Task CreateRequestAsync(
        Guid requestId,
        string commandName,
        CancellationToken cancellationToken)
    {
        var request = ProcessedRequest.Create(
            requestId,
            commandName,
            timeProvider.GetUtcNow());

        await context.Set<ProcessedRequest>().AddAsync(
            request,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}

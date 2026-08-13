using Boilerplate.Application.Common.Idempotency;

namespace Boilerplate.Application.Common.Behaviors;

public sealed class IdempotencyBehavior<TMessage, TResponse>(IRequestManager requestManager)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : ICommand<TResponse>, IIdempotentCommand
    where TResponse : Result, IResult<TResponse>
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (await requestManager.ExistsAsync(
                message.RequestId,
                cancellationToken))
        {
            return TResponse.Failure(IdempotencyErrors.DuplicateRequest(message.RequestId));
        }

        await requestManager.CreateRequestAsync(
            message.RequestId,
            typeof(TMessage).Name,
            cancellationToken);

        return await next(
            message,
            cancellationToken);
    }
}

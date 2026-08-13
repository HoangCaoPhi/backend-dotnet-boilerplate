using Boilerplate.Application.Common.Data;

namespace Boilerplate.Application.Common.Behaviors;

public sealed class UnitOfWorkBehavior<TMessage, TResponse>(IApplicationDbContext context)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : ICommand<TResponse>
    where TResponse : Result
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(
            message,
            cancellationToken);

        if (response.IsSuccess)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}

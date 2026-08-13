using FluentValidation;

namespace Boilerplate.Application.Common.Behaviors;

public sealed class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : ICommand<TResponse>
    where TResponse : Result, IResult<TResponse>
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(
                message,
                cancellationToken);
        }

        var context = new ValidationContext<TMessage>(message);

        var failures = (await Task.WhenAll(validators.Select(validator => validator.ValidateAsync(
                context,
                cancellationToken))))
            .SelectMany(result => result.Errors)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(
                message,
                cancellationToken);
        }

        var error = new Error(
            "Validation.Error",
            string.Join(
                " ",
                failures.Select(failure => failure.ErrorMessage)),
            ErrorType.Validation);

        return TResponse.Failure(error);
    }
}

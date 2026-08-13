using Boilerplate.Domain.TodoLists;
using FluentValidation;

namespace Boilerplate.Application.TodoLists.Commands.CreateTodoList;

public sealed class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
    public CreateTodoListCommandValidator()
    {
        RuleFor(command => command.RequestId).NotEmpty();
        RuleFor(command => command.Title).NotEmpty();

        RuleFor(command => command.ColourCode)
            .Must(code => code is null || Colour.IsSupported(code))
            .WithMessage(command => $"Colour \"{command.ColourCode}\" is not supported.");
    }
}

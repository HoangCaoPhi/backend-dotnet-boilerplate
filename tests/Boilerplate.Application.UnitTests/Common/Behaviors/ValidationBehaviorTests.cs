using Boilerplate.Application.Common.Behaviors;
using Boilerplate.Application.TodoLists.Commands.CreateTodoList;
using Boilerplate.SharedKernel.Results;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Boilerplate.Application.UnitTests.Common.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<CreateTodoListCommand, Result<Guid>>([]);
        var expected = Result<Guid>.Success(Guid.CreateVersion7());

        // Act
        var result = await behavior.Handle(
            CreateCommand(),
            (_, _) => new ValueTask<Result<Guid>>(expected),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ReturnsFailureWithoutCallingNext()
    {
        // Arrange
        var validator = Substitute.For<IValidator<CreateTodoListCommand>>();
        validator.ValidateAsync(
                Arg.Any<ValidationContext<CreateTodoListCommand>>(),
                Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Title", "Title is required.")]));

        var behavior = new ValidationBehavior<CreateTodoListCommand, Result<Guid>>([validator]);
        var nextCalled = false;

        // Act
        var result = await behavior.Handle(
            CreateCommand(),
            (_, _) =>
            {
                nextCalled = true;
                return new ValueTask<Result<Guid>>(Result<Guid>.Success(Guid.CreateVersion7()));
            },
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.Type.ShouldBe(ErrorType.Validation);
        nextCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsNext()
    {
        // Arrange
        var validator = Substitute.For<IValidator<CreateTodoListCommand>>();
        validator.ValidateAsync(
                Arg.Any<ValidationContext<CreateTodoListCommand>>(),
                Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<CreateTodoListCommand, Result<Guid>>([validator]);
        var expected = Result<Guid>.Success(Guid.CreateVersion7());

        // Act
        var result = await behavior.Handle(
            CreateCommand(),
            (_, _) => new ValueTask<Result<Guid>>(expected),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expected);
    }

    private static CreateTodoListCommand CreateCommand()
        => new(
            Guid.CreateVersion7(),
            "Groceries",
            null);
}

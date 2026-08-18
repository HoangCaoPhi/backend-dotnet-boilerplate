using Boilerplate.Application.Common.Behaviors;
using Boilerplate.Application.Common.Idempotency;
using Boilerplate.Application.TodoLists.Commands.CreateTodoList;
using Boilerplate.SharedKernel.Results;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Boilerplate.Application.UnitTests.Common.Behaviors;

public sealed class IdempotencyBehaviorTests
{
    private readonly IRequestManager _requestManager = Substitute.For<IRequestManager>();

    [Fact]
    public async Task Handle_DuplicateRequestId_ReturnsFailureWithoutCallingNext()
    {
        // Arrange
        _requestManager.ExistsAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(true);

        var behavior = new IdempotencyBehavior<CreateTodoListCommand, Result<Guid>>(_requestManager);
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
        result.Error.Type.ShouldBe(ErrorType.Conflict);
        nextCalled.ShouldBeFalse();
        await _requestManager.DidNotReceive().CreateRequestAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NewRequestId_RecordsRequestAndCallsNext()
    {
        // Arrange
        _requestManager.ExistsAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(false);

        var behavior = new IdempotencyBehavior<CreateTodoListCommand, Result<Guid>>(_requestManager);
        var command = CreateCommand();
        var expected = Result<Guid>.Success(Guid.CreateVersion7());

        // Act
        var result = await behavior.Handle(
            command,
            (_, _) => new ValueTask<Result<Guid>>(expected),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expected);
        await _requestManager.Received(1).CreateRequestAsync(
            command.RequestId,
            nameof(CreateTodoListCommand),
            Arg.Any<CancellationToken>());
    }

    private static CreateTodoListCommand CreateCommand()
        => new(
            Guid.CreateVersion7(),
            "Groceries",
            null);
}

using Boilerplate.Application.Common.Behaviors;
using Boilerplate.Application.Common.Data;
using Boilerplate.Application.TodoLists.Commands.CreateTodoList;
using Boilerplate.SharedKernel.Results;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Boilerplate.Application.UnitTests.Common.Behaviors;

public sealed class UnitOfWorkBehaviorTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();

    [Fact]
    public async Task Handle_SuccessResult_CommitsChanges()
    {
        // Arrange
        var behavior = new UnitOfWorkBehavior<CreateTodoListCommand, Result<Guid>>(_context);
        var expected = Result<Guid>.Success(Guid.CreateVersion7());

        // Act
        var result = await behavior.Handle(
            CreateCommand(),
            (_, _) => new ValueTask<Result<Guid>>(expected),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expected);
        await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FailureResult_DoesNotCommitChanges()
    {
        // Arrange
        var behavior = new UnitOfWorkBehavior<CreateTodoListCommand, Result<Guid>>(_context);
        var expected = Result<Guid>.Failure(new Error(
            "Test.Error",
            "failed",
            ErrorType.Validation));

        // Act
        var result = await behavior.Handle(
            CreateCommand(),
            (_, _) => new ValueTask<Result<Guid>>(expected),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expected);
        await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static CreateTodoListCommand CreateCommand()
        => new(
            Guid.CreateVersion7(),
            "Groceries",
            null);
}

namespace Boilerplate.Application.Common.Idempotency;

public interface IIdempotentCommand
{
    Guid RequestId { get; }
}

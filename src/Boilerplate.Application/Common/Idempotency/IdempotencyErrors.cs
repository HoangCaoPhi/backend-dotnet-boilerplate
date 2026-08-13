namespace Boilerplate.Application.Common.Idempotency;

public static class IdempotencyErrors
{
    public static Error DuplicateRequest(Guid requestId) => new(
        "Idempotency.DuplicateRequest",
        $"Request '{requestId}' was already processed.",
        ErrorType.Conflict);
}

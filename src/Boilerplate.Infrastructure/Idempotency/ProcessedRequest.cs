namespace Boilerplate.Infrastructure.Idempotency;

public sealed class ProcessedRequest
{
    public Guid Id { get; private set; }

    public string CommandName { get; private set; } = null!;

    public DateTimeOffset ProcessedOn { get; private set; }

    private ProcessedRequest()
    {
    }

    public static ProcessedRequest Create(
        Guid id,
        string commandName,
        DateTimeOffset processedOn)
        => new()
        {
            Id = id,
            CommandName = commandName,
            ProcessedOn = processedOn,
        };
}

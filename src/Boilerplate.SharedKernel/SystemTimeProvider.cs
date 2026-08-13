namespace Boilerplate.SharedKernel;

public sealed class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
}

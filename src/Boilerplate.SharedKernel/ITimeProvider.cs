namespace Boilerplate.SharedKernel;

public interface ITimeProvider
{
    DateTimeOffset GetUtcNow();
}

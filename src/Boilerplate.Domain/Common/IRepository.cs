namespace Boilerplate.Domain.Common;

public interface IRepository<T> where T : IAggregateRoot
{
    Task AddAsync(
        T aggregate,
        CancellationToken cancellationToken);
}

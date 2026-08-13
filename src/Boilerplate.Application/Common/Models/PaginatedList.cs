using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Application.Common.Models;

public sealed class PaginatedList<T>(
    IReadOnlyList<T> items,
    int count,
    int pageNumber,
    int pageSize)
{
    public IReadOnlyList<T> Items { get; } = items;

    public int PageNumber { get; } = pageNumber;

    public int TotalCount { get; } = count;

    public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(
            items,
            count,
            pageNumber,
            pageSize);
    }
}

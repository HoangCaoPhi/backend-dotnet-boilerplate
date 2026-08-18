using System.Text.Json;
using Boilerplate.Application.Common.IntegrationEvents;
using Boilerplate.Infrastructure.IntegrationEvents;
using Boilerplate.Infrastructure.Persistence;
using Boilerplate.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Boilerplate.Infrastructure.Outbox;

public sealed class OutboxProcessor(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OutboxProcessor> logger,
    ITimeProvider timeProvider) : BackgroundService
{
    private const int BatchSize = 20;
    private const int MaxAttempts = 5;

    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan Retention = TimeSpan.FromDays(7);
 
    public static readonly string ClaimSql = $"""
        SELECT * FROM "{OutboxMessageConfiguration.TableName}"
        WHERE "Status" IN ({(int)OutboxMessageStatus.Pending}, {(int)OutboxMessageStatus.Failed})
        ORDER BY "OccurredOn"
        LIMIT {BatchSize}
        FOR UPDATE SKIP LOCKED
        """;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
                await PurgeAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(
                    exception,
                    "Outbox processing cycle failed");
            }

            await Task.Delay(
                PollingInterval,
                stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IIntegrationEventPublisher>();

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var messages = await context.Set<OutboxMessage>()
            .FromSqlRaw(ClaimSql)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            return;
        }

        foreach (var message in messages)
        {
            await PublishAsync(
                publisher,
                message,
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        LogBacklog(messages);
    }

    private async Task PublishAsync(
        IIntegrationEventPublisher publisher,
        OutboxMessage message,
        CancellationToken cancellationToken)
    {
        message.MarkAttempted();

        try
        {
            await publisher.PublishAsync(
                Deserialize(message),
                message.Id,
                cancellationToken);

            message.MarkPublished(timeProvider.GetUtcNow());
        }
        catch (Exception exception)
        {
            message.MarkFailed(
                exception.Message,
                MaxAttempts);

            logger.LogError(
                exception,
                "Failed to publish outbox message {MessageId} ({MessageType}), attempt {Attempts}",
                message.Id,
                message.Type,
                message.Attempts);
        }
    }

    private async Task PurgeAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var threshold = timeProvider.GetUtcNow() - Retention;

        await context.Set<OutboxMessage>()
            .Where(message => message.Status == OutboxMessageStatus.Published && message.ProcessedOn < threshold)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private void LogBacklog(IReadOnlyCollection<OutboxMessage> messages)
    {
        var oldest = messages.Min(message => message.OccurredOn);

        logger.LogInformation(
            "Processed {Count} outbox messages, oldest was {Age} behind",
            messages.Count,
            timeProvider.GetUtcNow() - oldest);
    }

    private static IIntegrationEvent Deserialize(OutboxMessage message)
        => (IIntegrationEvent)(JsonSerializer.Deserialize(
                message.Content,
                IntegrationEventTypeRegistry.Resolve(message.Type))
            ?? throw new InvalidOperationException($"Failed to deserialize outbox message {message.Id}."));
}

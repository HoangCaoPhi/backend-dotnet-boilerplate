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
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                // A broker outage must kill only this cycle, not the whole background service.
                logger.LogError(
                    exception,
                    "Outbox processing cycle failed");
            }

            await Task.Delay(
                PollingInterval,
                stoppingToken);
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IIntegrationEventPublisher>();

        var messages = await context.Set<OutboxMessage>()
            .Where(message => message.ProcessedOn == null)
            .OrderBy(message => message.OccurredOn)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await publisher.PublishAsync(
                    message.Type,
                    message.Content,
                    cancellationToken);

                message.MarkProcessed(timeProvider.GetUtcNow());
            }
            catch (Exception exception)
            {
                message.MarkFailed(exception.Message);

                logger.LogError(
                    exception,
                    "Failed to publish outbox message {MessageId}",
                    message.Id);
            }
        }

        if (messages.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Boilerplate.Api.Common;
using Boilerplate.Domain.TodoLists;
using Boilerplate.Infrastructure.Outbox;
using Boilerplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shouldly;
using Xunit;

namespace Boilerplate.Api.IntegrationTests.TodoLists;

[Collection(ApiCollection.Name)]
public sealed class CompleteTodoItemFlowTests(ApiFactory apiFactory)
{
    private const string ExchangeName = "TodoItemCompletedEmailRequestedIntegrationEvent";
    private const string WebhookCommandName = "SendTodoItemCompletedWebhookCommand";

    [Fact]
    public async Task CompleteTodoItem_PublishesEmailRequestedIntegrationEvent()
    {
        var ct = TestContext.Current.CancellationToken;
        var (list, item) = await SeedListWithItemAsync("Flow test", ct);

        using var connection = await CreateConnectionAsync(ct);
        using var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        var queueName = await BindTemporaryQueueAsync(channel, ct);

        await CompleteItemAsync(list.Id, item.Id, ct);

        // The exchange is a fanout shared with every other test, so drain until our own item shows up.
        var message = await WaitForMessageAsync(
            channel,
            queueName,
            item.Id,
            ct)
            ?? throw new Exception("No message arrived. Outbox rows: " + await DescribeOutboxAsync(ct));

        var payload = JsonSerializer.Deserialize<JsonElement>(Encoding.UTF8.GetString(message.Body.ToArray()));
        payload.GetProperty("TodoListId").GetGuid().ShouldBe(list.Id);
        payload.GetProperty("TodoItemId").GetGuid().ShouldBe(item.Id);

        using var scope = apiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var row = await context.Set<OutboxMessage>().SingleAsync(
            outboxMessage => outboxMessage.Type == ExchangeName && outboxMessage.Content.Contains(item.Id.ToString()),
            ct);

        row.Status.ShouldBe(OutboxMessageStatus.Published);
        row.Attempts.ShouldBe(1);
        message.BasicProperties.MessageId.ShouldBe(row.Id.ToString());
        message.BasicProperties.Timestamp.UnixTime.ShouldBe(row.OccurredOn.ToUnixTimeSeconds());
    }

    [Fact]
    public async Task CompleteTodoItem_WebhookCommandKeepsFailing_EmailEventStillPublishesExactlyOnce()
    {
        var ct = TestContext.Current.CancellationToken;
        var (list, item) = await SeedListWithItemAsync("Isolation test", ct);

        using var connection = await CreateConnectionAsync(ct);
        using var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        var queueName = await BindTemporaryQueueAsync(channel, ct);

        await CompleteItemAsync(list.Id, item.Id, ct);

        // Slow: the webhook host never resolves, so this waits out several 5s retry cycles.
        await Task.Delay(
            TimeSpan.FromSeconds(14),
            ct);

        var received = 0;
        BasicGetResult? next;
        while ((next = await channel.BasicGetAsync(
                   queueName,
                   autoAck: true,
                   cancellationToken: ct)) is not null)
        {
            if (Encoding.UTF8.GetString(next.Body.ToArray()).Contains(item.Id.ToString()))
            {
                received++;
            }
        }

        received.ShouldBe(1);

        using var scope = apiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var rows = await context.Set<OutboxMessage>()
            .Where(outboxMessage => outboxMessage.Content.Contains(item.Id.ToString()))
            .ToListAsync(ct);

        rows.Count.ShouldBe(2);

        var email = rows.Single(row => row.Type == ExchangeName);
        email.Status.ShouldBe(OutboxMessageStatus.Published);
        email.Attempts.ShouldBe(1);

        var webhook = rows.Single(row => row.Type == WebhookCommandName);
        webhook.Status.ShouldBeOneOf(OutboxMessageStatus.Failed, OutboxMessageStatus.DeadLettered);
        webhook.Attempts.ShouldBeGreaterThan(1);
    }

    [Fact]
    public async Task OutboxClaim_SkipsRowsAlreadyLockedByAnotherWorker()
    {
        var ct = TestContext.Current.CancellationToken;

        using var firstScope = apiFactory.Services.CreateScope();
        using var secondScope = apiFactory.Services.CreateScope();
        var firstContext = firstScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var secondContext = secondScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // The background processor drains pending rows every 5s, so re-seed when it wins the race.
        for (var attempt = 0; attempt < 4; attempt++)
        {
            var (list, item) = await SeedListWithItemAsync($"Skip locked test {attempt}", ct);
            await CompleteItemAsync(list.Id, item.Id, ct);

            await using var firstTransaction = await firstContext.Database.BeginTransactionAsync(ct);
            var firstClaim = await ClaimAsync(firstContext, ct);

            if (firstClaim.Count == 0)
            {
                continue;
            }

            await using var secondTransaction = await secondContext.Database.BeginTransactionAsync(ct);
            var secondClaim = await ClaimAsync(secondContext, ct);

            secondClaim.Intersect(firstClaim).ShouldBeEmpty();
            return;
        }

        throw new Exception("The outbox never held a claimable row long enough to test locking.");
    }

    [Fact]
    public async Task CompleteTodoItem_WhenSaveChangesFails_RollsBackBothItemAndOutboxMessage()
    {
        var ct = TestContext.Current.CancellationToken;
        var (list, item) = await SeedListWithItemAsync("Atomic test", ct);

        using (var scope = apiFactory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var trackedList = await context.Set<TodoList>()
                .Include(l => l.Items)
                .SingleAsync(
                    l => l.Id == list.Id,
                    ct);
            trackedList.CompleteItem(item.Id);

            // Title exceeds the configured varchar(200) column: Postgres rejects this insert,
            // failing the whole SaveChanges call the item completion is part of.
            context.Set<TodoList>().Add(TodoList.Create(
                Guid.CreateVersion7(),
                new string('x', 201)));

            await Should.ThrowAsync<DbUpdateException>(() => context.SaveChangesAsync(ct));
        }

        using (var scope = apiFactory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var persistedItem = await context.Set<TodoItem>().SingleAsync(
                i => i.Id == item.Id,
                ct);
            persistedItem.IsDone.ShouldBeFalse();

            var outboxRows = await context.Set<OutboxMessage>()
                .Where(outboxMessage => outboxMessage.Content.Contains(item.Id.ToString()))
                .ToListAsync(ct);
            outboxRows.ShouldBeEmpty();
        }
    }

    private static async Task<BasicGetResult?> WaitForMessageAsync(
        IChannel channel,
        string queueName,
        Guid todoItemId,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            await Task.Delay(1000, cancellationToken);

            BasicGetResult? message;
            while ((message = await channel.BasicGetAsync(
                       queueName,
                       autoAck: true,
                       cancellationToken: cancellationToken)) is not null)
            {
                if (Encoding.UTF8.GetString(message.Body.ToArray()).Contains(todoItemId.ToString()))
                {
                    return message;
                }
            }
        }

        return null;
    }

    private static async Task<List<Guid>> ClaimAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var claimed = await context.Set<OutboxMessage>()
            .FromSqlRaw(OutboxProcessor.ClaimSql)
            .ToListAsync(cancellationToken);

        return claimed.Select(message => message.Id).ToList();
    }

    private async Task<(TodoList List, TodoItem Item)> SeedListWithItemAsync(
        string name,
        CancellationToken cancellationToken)
    {
        var list = TodoList.Create(
            Guid.CreateVersion7(),
            $"{name} list");
        var item = list.AddItem(
            Guid.CreateVersion7(),
            $"{name} item",
            null,
            PriorityLevel.Low,
            null,
            null);

        using var scope = apiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Set<TodoList>().Add(list);
        await context.SaveChangesAsync(cancellationToken);

        return (list, item);
    }

    private async Task CompleteItemAsync(
        Guid listId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        using var client = apiFactory.CreateClient();
        var configuration = apiFactory.Services.GetRequiredService<IConfiguration>();
        var token = JwtTokenFactory.CreateToken(
            configuration,
            Guid.CreateVersion7());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsync(
            $"/api/todo-lists/{listId}/items/{itemId}/complete",
            null,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private async Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = apiFactory.RabbitMqUri.Host,
            Port = apiFactory.RabbitMqUri.Port,
            UserName = apiFactory.RabbitMqUri.UserInfo.Split(':')[0],
            Password = apiFactory.RabbitMqUri.UserInfo.Split(':')[1],
        };

        return await connectionFactory.CreateConnectionAsync(cancellationToken);
    }

    private static async Task<string> BindTemporaryQueueAsync(
        IChannel channel,
        CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            ExchangeName,
            ExchangeType.Fanout,
            durable: true,
            cancellationToken: cancellationToken);

        var queue = await channel.QueueDeclareAsync(cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue.QueueName,
            ExchangeName,
            string.Empty,
            cancellationToken: cancellationToken);

        return queue.QueueName;
    }

    private async Task<string> DescribeOutboxAsync(CancellationToken cancellationToken)
    {
        using var scope = apiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var rows = await context.Set<OutboxMessage>().ToListAsync(cancellationToken);

        return string.Join(
            " | ",
            rows.Select(row => $"Type={row.Type} Status={row.Status} Attempts={row.Attempts} Error={row.Error}"));
    }
}

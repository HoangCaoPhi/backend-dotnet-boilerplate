using System.Text;
using System.Text.Json;
using Boilerplate.Application.Common.IntegrationEvents;
using RabbitMQ.Client;

namespace Boilerplate.Infrastructure.IntegrationEvents;

public sealed class RabbitMqIntegrationEventPublisher(IConnection connection)
    : IIntegrationEventPublisher, IAsyncDisposable
{
    private static readonly CreateChannelOptions ChannelOptions = new(
        publisherConfirmationsEnabled: true,
        publisherConfirmationTrackingEnabled: true);

    private readonly HashSet<string> _declaredExchanges = [];

    private IChannel? _channel;

    public async Task PublishAsync(
        IIntegrationEvent integrationEvent,
        Guid messageId,
        CancellationToken cancellationToken)
    {
        var exchangeName = integrationEvent.GetType().Name;
        var channel = await GetChannelAsync(cancellationToken);
        await DeclareExchangeAsync(
            channel,
            exchangeName,
            cancellationToken);

        var properties = new BasicProperties
        {
            Persistent = true,
            MessageId = messageId.ToString(),
            Type = exchangeName,
            ContentType = "application/json",
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
            integrationEvent,
            integrationEvent.GetType()));

        await channel.BasicPublishAsync(
            exchangeName,
            routingKey: string.Empty,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
        => _channel ??= await connection.CreateChannelAsync(
            ChannelOptions,
            cancellationToken);

    private async Task DeclareExchangeAsync(
        IChannel channel,
        string exchangeName,
        CancellationToken cancellationToken)
    {
        if (!_declaredExchanges.Add(exchangeName))
        {
            return;
        }

        await channel.ExchangeDeclareAsync(
            exchangeName,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
    }
}

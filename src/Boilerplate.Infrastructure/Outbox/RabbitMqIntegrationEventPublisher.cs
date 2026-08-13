using System.Text;
using RabbitMQ.Client;

namespace Boilerplate.Infrastructure.Outbox; 

public sealed class RabbitMqIntegrationEventPublisher(IConnection connection) : IIntegrationEventPublisher
{
    public async Task PublishAsync(
        string eventType,
        string content,
        CancellationToken cancellationToken)
    {
        var exchangeName = ToExchangeName(eventType);

        using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchangeName,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(content);

        await channel.BasicPublishAsync(
            exchangeName,
            string.Empty,
            mandatory: false,
            basicProperties: new BasicProperties { Persistent = true },
            body: body,
            cancellationToken: cancellationToken);
    }
     
    private static string ToExchangeName(string eventType)
        => eventType
            .Split(',')[0]
            .Split('.')[^1];
}

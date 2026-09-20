using System.Reflection;
using System.Text;
using System.Text.Json;
using Backend.Src.Domain.Attributes.MessageBroker;
using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Infrastructure.Options.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Backend.Src.Infrastructure.MessageBroker.RabbitMQ;

public class RabbitMqProducerAdapter(
    IConnection? connection,
    IOptions<RabbitMqOptions> options,
    IServiceScopeFactory scopeFactory
) : IMessageProducerPort
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : IntegrationMessage
    {
        if (connection is null)
        {
            Console.WriteLine($"[RabbitMQ] Connection unavailable. Dispatching {typeof(TMessage).Name} directly in-process...");
            await using var scope = scopeFactory.CreateAsyncScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumerPort<TMessage>>();
            await consumer.ConsumeAsync(message, cancellationToken);
            return;
        }

        Console.WriteLine("Producer configurating...");
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var route = typeof(TMessage).GetCustomAttribute<MessageRouteAttribute>()
            ?? throw new InvalidOperationException($"Message {typeof(TMessage).Name} has no message route.");

        string routingKey = route?.RoutingKey ?? typeof(TMessage).Name;
        string exchangeType = route?.ExchangeType ?? ExchangeType.Direct;

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: exchangeType,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        string jsonPayload = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(jsonPayload);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
        
        Console.WriteLine($"Produces has produced a request to {routingKey}");
    }
}

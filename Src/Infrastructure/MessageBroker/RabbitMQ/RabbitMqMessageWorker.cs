using System.Reflection;
using System.Text.Json;
using Backend.Src.Domain.Attributes.MessageBroker;
using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Infrastructure.Options.Common;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Backend.Src.Infrastructure.MessageBroker.RabbitMQ;

public class RabbitMqMessageWorker<TMessage>(
    IConnection? connection,
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqOptions> options
) : BackgroundService where TMessage : IntegrationMessage
{
    private readonly RabbitMqOptions _options = options.Value;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (connection is null)
        {
            Console.WriteLine($"[RabbitMQ] Worker for {typeof(TMessage).Name} skipped: message broker connection is unavailable.");
            return;
        }

        Console.WriteLine("Configurating Exchange and Queue...");
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var route = typeof(TMessage).GetCustomAttribute<MessageRouteAttribute>()
            ?? throw new InvalidOperationException($"Message {typeof(TMessage).Name} has no MessageRouteAttribute.");

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: route.ExchangeType,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        await channel.QueueDeclareAsync(
            queue: route.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        await channel.QueueBindAsync(
            queue: route.QueueName,
            exchange: _options.ExchangeName,
            routingKey: route.RoutingKey,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        
        Console.WriteLine("Exchange and Queue configured");

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<TMessage>(ea.Body.Span, JsonOptions)
                    ?? throw new InvalidOperationException($"Unable to deserialize {typeof(TMessage).Name}.");

                await using var scope = scopeFactory.CreateAsyncScope();

                Console.WriteLine($"Sending message to Consumer {typeof(TMessage).Name}");

                var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumerPort<TMessage>>();
                await consumer.ConsumeAsync(message, stoppingToken);

                await channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken
                );
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Leave the delivery unacknowledged. RabbitMQ will requeue it
                // when the channel closes during the graceful shutdown.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error ocurred while sending message to consumer: {ex}");
                await channel.BasicNackAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken
                );
            }
        };

        await channel.BasicConsumeAsync(
            queue: route.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

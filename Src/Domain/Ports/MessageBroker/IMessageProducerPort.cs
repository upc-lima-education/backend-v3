using Backend.Src.Domain.Contracts.Common;

namespace Backend.Src.Domain.Ports.MessageBroker;

public interface IMessageProducerPort
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IntegrationMessage;
}
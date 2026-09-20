using Backend.Src.Domain.Contracts.Common;

namespace Backend.Src.Domain.Ports.MessageBroker;

public interface IMessageConsumerPort<TMessage> where TMessage : IntegrationMessage
{
    Task ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
}
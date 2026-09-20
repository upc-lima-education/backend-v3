namespace Backend.Src.Domain.Attributes.MessageBroker;

[AttributeUsage(AttributeTargets.Class)]
public class MessageRouteAttribute(string routingKey, string queueName, string exchangeType) : Attribute
{
    public string RoutingKey { get; } = routingKey;
    public string QueueName { get; } = queueName;
    public string ExchangeType { get; } = exchangeType;
}
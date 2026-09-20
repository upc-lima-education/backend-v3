using Backend.Src.Application.Dtos.Enums.Notifications;
using Backend.Src.Domain.Attributes.MessageBroker;
using Backend.Src.Domain.Contracts.Common;

namespace Backend.Src.Domain.Contracts.MessageBroker.Recruitment;

[MessageRoute(
    routingKey: "recruitment.application.rejected",
    queueName: "application-rejected-notifications",
    exchangeType: "direct"
)]
public record RejectJobApplicationMessage(
    Guid CandidateId,
    string JobTitle,
    string CompanyName,
    IReadOnlyCollection<NotificationChannel> Channels
) : IntegrationMessage(Guid.NewGuid(), DateTime.UtcNow);
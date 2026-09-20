using Backend.Src.Domain.Attributes.MessageBroker;
using Backend.Src.Domain.Contracts.Common;

namespace Backend.Src.Domain.Contracts.MessageBroker.Curriculums;

[MessageRoute(routingKey: "cv.improvement.requested", queueName: "cv-improvement", exchangeType: "direct")]
public record AiAssistedCvImprovementMessage(
    Guid CvId,
    Guid? JobId,
    Guid UserId,
    List<AiAssistedCvImprovementOption> Options
) : IntegrationMessage(Guid.NewGuid(), DateTime.UtcNow);

public enum AiAssistedCvImprovementOption
{
    Summary,
    WorkExperience,
    Certification,
    Project,
    Award
}
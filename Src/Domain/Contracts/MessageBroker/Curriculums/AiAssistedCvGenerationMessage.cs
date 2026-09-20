using Backend.Src.Domain.Attributes.MessageBroker;
using Backend.Src.Domain.Contracts.Common;

namespace Backend.Src.Domain.Contracts.MessageBroker.Curriculums;

[MessageRoute(routingKey: "cv.generation.requested", queueName: "cv-generation", exchangeType: "direct")]
public record AiAssistedCvGenerationMessage(
    Guid CvId,
    Guid? JobId,
    Guid UserId
) : IntegrationMessage(Guid.NewGuid(), DateTime.UtcNow);
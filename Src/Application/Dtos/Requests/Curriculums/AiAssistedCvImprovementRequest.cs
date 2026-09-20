using Backend.Src.Domain.Contracts.MessageBroker.Curriculums;

namespace Backend.Src.Application.Dtos.Requests.Curriculums;

public record AiAssistedCvImprovementRequest(
    Guid CvId,
    Guid? JobId,
    List<AiAssistedCvImprovementOption> Options
);
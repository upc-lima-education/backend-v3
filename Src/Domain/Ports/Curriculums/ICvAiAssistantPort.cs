using Backend.Src.Domain.Contracts.Curriculums;

namespace Backend.Src.Domain.Ports.Curriculums;

public interface ICvAiAssistantPort
{
    Task<AiAssistedCvGenerationResponse> GenerateAsync(AiAssistedCvGenerationRequest request, CancellationToken cancellationToken = default);
    Task<AiAssistedCvImprovementResponse> ImproveAsync(AiAssistedCvImprovementRequest request, CancellationToken cancellationToken = default);
}
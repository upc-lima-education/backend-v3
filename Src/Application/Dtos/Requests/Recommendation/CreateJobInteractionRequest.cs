using Backend.Src.Domain.ValueObjects.Recommendation;

namespace Backend.Src.Application.Dtos.Requests.Recommendation;

public sealed record CreateJobInteractionRequest(Guid JobId, JobInteractionType Type);

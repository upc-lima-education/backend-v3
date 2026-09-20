using Backend.Src.Application.Dtos.Responses.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Curriculums;

public class GetMyCvsUseCase(
    ICvRepository cvRepository,
    IProfileRepository profileRepository
)
{
    public async Task<IReadOnlyList<CvSummaryResponse>> ExecuteAsync(Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);

        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var cvs = await cvRepository.GetAllByCandidateIdAsync(profile.Id);

        var responses = cvs
            .OrderByDescending(c => c.IsCurrent)
            .ThenByDescending(c => c.UpdatedAt)
            .Select(c => new CvSummaryResponse(
                c.Id,
                c.Title,
                c.IsCurrent,
                !string.IsNullOrEmpty(c.FileContentKey),
                c.ProcessingStatus.ToString(),
                c.ProcessingError,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .ToList();

        return responses;
    }
}

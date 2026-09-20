using Backend.Src.Application.Dtos.Responses.Curriculums;
using Backend.Src.Domain.Exceptions.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Curriculums;

public class GetCvProcessingStatusUseCase(
    ICvRepository cvRepository,
    IProfileRepository profileRepository
)
{
    public async Task<CvProcessingStatusResponse> ExecuteAsync(Guid cvId, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var cv = await cvRepository.GetByIdAsync(cvId)
            ?? throw new CvNotFoundException(cvId);
        if (cv.CandidateId != profile.Id)
            throw new CvAccessDeniedException(cv.Id);

        return new CvProcessingStatusResponse(
            cv.Id,
            cv.ProcessingStatus.ToString(),
            cv.ProcessingError
        );
    }
}

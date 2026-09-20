using Backend.Src.Application.Dtos.Responses.Curriculums;
using Backend.Src.Application.Mappers.Curriculums;
using Backend.Src.Domain.Exceptions.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Application.UseCases.Curriculums;

public class GetStructuredCvUseCase(
    ICvRepository cvRepository,
    IProfileRepository profileRepository,
    ICvStructuredContentRepository structuredContentRepository
)
{
    public async Task<CvStructuredContentResponse> ExecuteAsync(Guid cvId, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null) throw new CandidateProfileRequiredException();

        var cv = await cvRepository.GetByIdAsync(cvId)
            ?? throw new CvNotFoundException(cvId);
        if (cv.CandidateId != profile.Id) throw new CvAccessDeniedException(cv.Id);

        var content = await structuredContentRepository.GetByIdAsync(cv.Id)
            ?? throw new CvStructuredContentNotFoundException(cvId);

        return CvStructuredContentMapper.ToResponse(content);
    }
}
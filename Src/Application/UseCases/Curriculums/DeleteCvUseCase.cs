using Backend.Src.Domain.Exceptions.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Curriculums;

public class DeleteCvUseCase(
    ICvRepository cvRepository,
    IProfileRepository profileRepository,
    ICvStructuredContentRepository structuredContentRepository,
    IFileStoragePort fileStoragePort
)
{
    public async Task ExecuteAsync(Guid id, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);

        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var cv = await cvRepository.GetByIdAsync(id)
            ?? throw new CvNotFoundException(id);

        if (cv.CandidateId != profile.Id)
            throw new CvAccessDeniedException(cv.Id);

        await structuredContentRepository.DeleteAsync(cv.Id);
        if (!string.IsNullOrWhiteSpace(cv.FileContentKey))
            await fileStoragePort.DeleteAsync(cv.FileContentKey);

        await cvRepository.DeleteAsync(cv.Id);
    }
}
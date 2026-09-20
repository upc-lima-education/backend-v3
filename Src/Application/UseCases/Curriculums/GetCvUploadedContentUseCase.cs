using Backend.Src.Application.Dtos.Responses.Curriculums;
using Backend.Src.Domain.Exceptions.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Rules.Curriculums;
using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Application.UseCases.Curriculums;

public class GetCvUploadedContentUseCase(
    ICvRepository cvRepository,
    IProfileRepository profileRepository,
    IFileStoragePort fileStorage
)
{
    public async Task<GetCvUploadedResponse> ExecuteAsync(Guid cvId, Guid userId)
    {
        var cv = await cvRepository.GetByIdAsync(cvId)
            ?? throw new CvNotFoundException(cvId);

        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (cv.CandidateId != profile.Id)
            throw new CvAccessDeniedException(cv.Id);
        
        if (string.IsNullOrWhiteSpace(cv.FileContentKey))
            throw new CvUploadedContentNotFoundException(cvId);

        if (!Path.GetExtension(cv.FileContentKey).Equals(CvUploadedContentRules.PdfExtension, StringComparison.OrdinalIgnoreCase))
            throw new CvPdfRequiredException(cvId);

        var stream = await fileStorage.DownloadAsync(cv.FileContentKey);

        return new GetCvUploadedResponse(
            stream,
            CvUploadedContentRules.PdfContentType,
            $"{cv.Title}.pdf"
        );
    }
}

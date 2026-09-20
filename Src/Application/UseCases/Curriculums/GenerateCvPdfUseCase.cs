using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Exceptions.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Ports.Curriculums;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Application.UseCases.Curriculums;

public class GenerateCvPdfUseCase(
    ICvRepository cvRepository,
    IProfileRepository profileRepository,
    ICvStructuredContentRepository structuredContentRepository,
    ICvHtmlRendererPort htmlRenderer,
    IPdfRendererPort pdfRenderer,
    IFileStoragePort fileStorage
)
{
    public async Task<string> ExecuteAsync(Guid cvId, Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();
        
        var cv = await cvRepository.GetByIdAsync(cvId)
            ?? throw new CvNotFoundException(cvId);
        
        if (cv.CandidateId != profile.Id)
            throw new CvAccessDeniedException(cv.Id);
        if (cv.ProcessingStatus == CvProcessingStatus.Processing)
            throw new CvProcessingInProgressException(cv.Id);

        var structuredContent = await structuredContentRepository.GetByIdAsync(cv.Id)
            ?? throw new CvStructuredContentNotFoundException(cv.Id);
        
        var html = htmlRenderer.Render(structuredContent);

        await using var pdfStream = await pdfRenderer.RenderAsync(html, cancellationToken);

        var storageKey = $"cvs/{cv.CandidateId}/{cv.Id}.pdf";

        var uploadRequest = new StorageUploadRequest(
            storageKey,
            pdfStream,
            "application/pdf"
        );
        var result = await fileStorage.UploadAsync(uploadRequest);
        cv.SetFileContent(storageKey);
        await cvRepository.UpdateAsync(cv);
        return result.StorageKey;
    }
}

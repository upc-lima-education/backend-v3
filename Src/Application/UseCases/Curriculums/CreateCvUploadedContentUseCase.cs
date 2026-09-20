using Backend.Src.Application.Dtos.Requests.Curriculums;
using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Curriculums;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Curriculums;

public class CreateCvUploadedContentUseCase(
    ICvRepository cvRepository,
    IProfileRepository profileRepository,
    IFileStoragePort fileStorage,
    IValidator<CreateCvUploadedContentRequest> validator
)
{
    public async Task<Guid> ExecuteAsync(CreateCvUploadedContentRequest request, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var cv = new Cv(
            profile.Id,
            request.Title,
            request.IsCurrent
        );
        
        var extension = Path.GetExtension(request.Cv.File.FileName).ToLowerInvariant();
        var storageKey = $"cvs/{profile.Id}/{cv.Id}{extension}";
        try
        {
            var storageRequest = new StorageUploadRequest(
                storageKey,
                request.Cv.File.Content,
                request.Cv.File.ContentType
            );
            await fileStorage.UploadAsync(storageRequest);
            cv.SetFileContent(storageKey);
            await cvRepository.CreateAsync(cv);
            return cv.Id;
        }
        catch
        {
            await fileStorage.DeleteAsync(storageKey);
            throw;
        }
    }
}
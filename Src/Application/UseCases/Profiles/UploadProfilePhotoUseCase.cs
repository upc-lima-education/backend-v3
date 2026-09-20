using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Contracts.Common;
using FluentValidation;
using Backend.Src.Application.Dtos.Responses.Profiles;
using Backend.Src.Application.Dtos.Requests.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class UploadProfilePictureUseCase(
    IProfileRepository profiles,
    IFileStoragePort storage,
    IValidator<UploadProfilePictureRequest> validator
)
{
    public async Task<UploadProfilePictureResponse> ExecuteAsync(UploadProfilePictureRequest request, Guid userId)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var profile = await profiles.GetByUserIdForUpdateAsync(userId)
            ?? throw new ProfileNotFoundException(userId);

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        var storageKey = $"profiles/{userId}/profile-picture{extension}";

        var storageRequest = new StorageUploadRequest(
            storageKey,
            request.File.Content,
            request.File.ContentType
        );

        var storedFile = await storage.UploadAsync(storageRequest);
        profile.UpdateProfilePicture(storedFile.StorageKey);
        await profiles.UpdateAsync(profile);
        return UploadProfilePictureResponseMapper.ToResponse(profile);
    }
}

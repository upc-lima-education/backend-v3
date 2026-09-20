using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Repositories.Profiles;
using FluentValidation;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Dtos.Responses.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class CreateOrganizationProfileUseCase(
    IProfileRepository profileRepository,
    IFileStoragePort storagePort,
    IValidator<CreateCompanyProfileRequest> validator
)
{
    public async Task<ProfileResponse> ExecuteAsync(CreateCompanyProfileRequest request, Guid userId)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        Profile profile;
        var existingProfile = await profileRepository.GetByUserIdForUpdateAsync(userId);
        if (existingProfile is not null)
        {
            if (existingProfile.CompanyProfile is not null)
                throw new CompanyProfileAlreadyExistsException(existingProfile.Id);
            if (existingProfile.CandidateProfile is not null)
                throw new CandidateProfileAlreadyExistsException(existingProfile.Id);

            profile = existingProfile;
            profile.Update(
                request.Description,
                request.Ubigeo,
                request.PhoneNumber,
                null
            );
        }
        else profile = ProfileMapper.ToEntity(request, userId);

        profile.CompanyProfile = CompanyProfileMapper.ToEntity(request, profile);

        if (request.ProfilePicture is not null)
        {
            var extension = Path.GetExtension(request.ProfilePicture.File.FileName).ToLowerInvariant();
            var storageKey = $"profiles/{userId}/profile-picture{extension}";
            var storageRequest = new StorageUploadRequest(
                storageKey,
                request.ProfilePicture.File.Content,
                request.ProfilePicture.File.ContentType
            );
            var storageResponse = await storagePort.UploadAsync(storageRequest);
            profile.UpdateProfilePicture(storageResponse.StorageKey);
        }
        
        if (existingProfile is not null) await profileRepository.UpdateAsync(profile);
        else await profileRepository.CreateAsync(profile);
        var response = ProfileMapper.ToResponse(profile);
        return response;
    }
}

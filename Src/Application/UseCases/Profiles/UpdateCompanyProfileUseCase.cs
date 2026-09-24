using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Domain.Repositories.Profiles;
using FluentValidation;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Dtos.Responses.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class UpdateCompanyProfileUseCase(
    IProfileRepository profileRepository,
    IValidator<UpdateCompanyProfileRequest> validator
)
{
    public async Task<ProfileResponse> ExecuteAsync(UpdateCompanyProfileRequest request, Guid userId)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var profile = await profileRepository.GetByUserIdForUpdateAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CompanyProfile is null)
            throw new CompanyProfileRequiredException();

        profile.Update(
            request.Description,
            request.Ubigeo,
            request.PhoneNumber,
            null //Skills
        );

        var rucToUpdate = request.Ruc != null
            ? (string.IsNullOrWhiteSpace(request.Ruc) ? null : request.Ruc.Trim())
            : profile.CompanyProfile.Ruc;

        profile.CompanyProfile.Update(
            request.CompanyName ?? profile.CompanyProfile.CompanyName,
            request.Sector ?? profile.CompanyProfile.Sector,
            request.Website ?? profile.CompanyProfile.Website,
            request.CompanySize ?? profile.CompanyProfile.CompanySize,
            rucToUpdate
        );

        await profileRepository.UpdateAsync(profile);

        var response = ProfileMapper.ToResponse(profile);
        return response;
    }
}

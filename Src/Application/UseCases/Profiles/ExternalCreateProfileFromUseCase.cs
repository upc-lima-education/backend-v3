using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class ExternalCreateProfileUseCase(IProfileRepository profileRepository)
{
    public async Task<Profile> ExecuteAsync(ExternalCreateProfileRequest request)
    {
        var existingProfile = await profileRepository.GetByUserIdForUpdateAsync(request.UserId);
        if (existingProfile is not null)
        {
            if (existingProfile.CompanyProfile is not null)
                throw new CompanyProfileAlreadyExistsException(existingProfile.Id);

            if (existingProfile.CandidateProfile is not null)
                throw new CandidateProfileAlreadyExistsException(existingProfile.Id);
        }

        var profile = new Profile(
            request.UserId,
            null, //Description
            null, // Ubigeo
            request.ProfilePicture,
            null, //Phone Number
            null //Skills
        );

        var profileType = request.ProfileType ?? ProfileType.Candidate;

        if (profileType is ProfileType.Company)
        {
            var companyName = $"{request.FirstName} {request.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(companyName))
                companyName = "Mi Empresa";

            profile.CompanyProfile = new CompanyProfile(
                profile.Id,
                profile,
                companyName,
                string.Empty, //Sector
                string.Empty, //Ruc
                null, //Website
                null //CompanySize
            );
        }
        else
        {
            profile.CandidateProfile = new CandidateProfile(
                profile.Id,
                profile,
                request.FirstName,
                request.LastName,
                string.Empty //dni
            );
        }

        if (existingProfile is not null) await profileRepository.UpdateAsync(profile);
        else await profileRepository.CreateAsync(profile);

        return profile;
    }
}

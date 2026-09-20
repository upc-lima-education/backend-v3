using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class VerifyCompanyProfileUseCase(IProfileRepository profileRepository)
{
    public async Task ExecuteAsync(Guid profileId, Guid userId)
    {
        var profile = await profileRepository.GetByIdForUpdateAsync(profileId)
            ?? throw new ProfileNotFoundException(profileId);
        if (profile.CompanyProfile is null)
            throw new CompanyProfileRequiredException();

        if (profile.UserId == userId)
            throw new SelfCompanyVerificationForbiddenException();

        profile.CompanyProfile.VerifyCompany(userId);
        await profileRepository.UpdateAsync(profile);
    }
}
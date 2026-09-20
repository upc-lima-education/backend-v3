using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Application.Resolvers.Skills;
using Backend.Src.Domain.Repositories.Profiles;
using FluentValidation;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Dtos.Responses.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class UpdateCandidateProfileUseCase(
    IProfileRepository profileRepository,
    SkillResolver skillResolver,
    IValidator<UpdateCandidateProfileRequest> validator
)
{
    public async Task<ProfileResponse> ExecuteAsync(UpdateCandidateProfileRequest request, Guid userId)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var profile = await profileRepository.GetByUserIdForUpdateAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var skills = await skillResolver.ResolveAsync(request.Skills ?? []);

        profile.Update(
            request.Description ?? profile.Description ?? "",
            request.Ubigeo ?? profile.Ubigeo ?? "",
            request.PhoneNumber ?? profile.PhoneNumber ?? "",
            skills
        );

        profile.CandidateProfile.Update(
            request.FirstName ?? profile.CandidateProfile.FirstName,
            request.LastName ?? profile.CandidateProfile.LastName,
            request.Dni ?? profile.CandidateProfile.Dni
        );

        await profileRepository.UpdateAsync(profile);

        var response = ProfileMapper.ToResponse(profile);
        return response;
    }
}
using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Profiles;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Auth;

public class UpdateContactUseCase(IProfileRepository profileRepository, IValidator<UpdateContactRequest> validator)
{
    public async Task ExecuteAsync(UpdateContactRequest request, Guid userId)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var profile = await profileRepository.GetByUserIdForUpdateAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        profile.Update(profile.Description, profile.Ubigeo, request.PhoneNumber?.Trim(), null);
        await profileRepository.UpdateAsync(profile);
    }
}

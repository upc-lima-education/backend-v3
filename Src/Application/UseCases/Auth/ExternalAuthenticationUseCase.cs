using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.Dtos.Responses.Auth;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Auth;

public class ExternalAuthenticationUseCase(
    IExternalAuthenticationPort externalAuthentication,
    IUserRepository userRepository,
    ExternalSignInUseCase signInUseCase,
    ExternalSignUpUseCase signUpUseCase
)
{
    public async Task<AuthenticationResponse?> ExecuteAsync(ExternalAuthenticationRequest request)
    {
        var tokenId = await externalAuthentication.GetExchangeCodeAsync(request.Code);
        if (tokenId is null || string.IsNullOrEmpty(tokenId))
            throw new ExternalTokenIdNotFoundException();

        var userIdentity = await externalAuthentication.GetUserIdentityAsync(tokenId)
            ?? throw new ExternalUserIdentityNotFoundException();

        var existingUser = await userRepository.GetByEmailAsync(userIdentity.Email);
        if (string.Equals(request.Mode, "login", StringComparison.OrdinalIgnoreCase))
        {
            if (existingUser is null) throw new InvalidCredentialsException();
            return await signInUseCase.ExecuteAsync(existingUser);
        }

        if (!string.Equals(request.Mode, "signup", StringComparison.OrdinalIgnoreCase) || request.ProfileType is null)
            throw new ValidationException("Google signup requires a valid profile type.");
        if (existingUser is not null) throw new EmailAlreadyExistsException(userIdentity.Email);

        return await signUpUseCase.ExecuteAsync(userIdentity, request.ProfileType.Value);
    }
}

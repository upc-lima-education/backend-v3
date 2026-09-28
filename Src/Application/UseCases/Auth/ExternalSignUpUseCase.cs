using Backend.Src.Application.Dtos.Data.Auth;
using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Dtos.Responses.Auth;
using Backend.Src.Application.Mappers.Auth;
using Backend.Src.Application.UseCases.Profiles;
using Backend.Src.Domain.Contracts.Auth;
using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;

namespace Backend.Src.Application.UseCases.Auth;

public class ExternalSignUpUseCase(
    IUserRepository userRepository,
    CreateProfileUseCase bootstrapProfileUseCase,
    IJwtPort jwtService,
    IRefreshTokenRepository refreshTokenRepository
)
{
    public async Task<AuthenticationResponse?> ExecuteAsync(ExternalUserIdentity userIdentity, ProfileType profileType)
    {
        var user = new User(
            userIdentity.Email,
            userIdentity.IsEmailVerified,
            null
        );
        await userRepository.CreateAsync(user);

        var createProfileRequest = new CreateProfileRequest(user.Id, profileType);
        var profile = await bootstrapProfileUseCase.ExecuteAsync(createProfileRequest);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken(user.Id);
        await refreshTokenRepository.SaveAsync(
            user.Id,
            refreshToken.Jti,
            refreshToken.Expiration
        );

        var userData = UserResponseMapper.ToData(user, profileType, profile.Id);
        var suggestedProfileData = new SuggestedProfileData(
            userIdentity.FirstName,
            userIdentity.LastName,
            userIdentity.PictureUrl
        );
        var response = new AuthenticationResponse(
            userData,
            accessToken,
            refreshToken.Token,
            1800,
            suggestedProfileData
        );
        return response;
    }
}

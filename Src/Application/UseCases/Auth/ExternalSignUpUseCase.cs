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
    ExternalCreateProfileUseCase bootstrapProfileUseCase,
    IJwtPort jwtService,
    IRefreshTokenRepository refreshTokenRepository
)
{
    public async Task<AuthenticationResponse?> ExecuteAsync(ExternalUserIdentity userIdentity, ProfileType? profileType = null)
    {
        var effectiveProfileType = profileType ?? ProfileType.Candidate;

        var user = new User(
            userIdentity.Email,
            userIdentity.IsEmailVerified,
            null
        );
        await userRepository.CreateAsync(user);

        var profile = await bootstrapProfileUseCase.ExecuteAsync(
            new ExternalCreateProfileRequest(
                user.Id,
                effectiveProfileType,
                userIdentity.FirstName ?? string.Empty,
                userIdentity.LastName ?? string.Empty,
                userIdentity.PictureUrl ?? string.Empty
            )
        );

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken(user.Id);
        await refreshTokenRepository.SaveAsync(
            user.Id,
            refreshToken.Jti,
            refreshToken.Expiration
        );

        var userResponse = UserResponseMapper.ToResponse(user, effectiveProfileType.ToString(), profile.Id);
        var response = new AuthenticationResponse(
            userResponse,
            accessToken,
            refreshToken.Token,
            1800,
            new SuggestedProfileData(
                userIdentity.FirstName,
                userIdentity.LastName,
                userIdentity.PictureUrl
            )
        );
        return response;
    }
}

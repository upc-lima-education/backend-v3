using Backend.Src.Application.Dtos.Responses.Auth;
using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Mappers.Auth;
using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Auth;

public class ExternalSignInUseCase(
    IProfileRepository profileRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtPort jwtService
)
{
    public async Task<AuthenticationResponse?> ExecuteAsync(User user)
    {
        var profile = await profileRepository.GetByUserIdAsync(user.Id);
        string? profileType = null;

        if (profile?.CandidateProfile is not null)
            profileType = ProfileType.Candidate.ToString();

        if (profile?.CompanyProfile is not null)
            profileType = ProfileType.Company.ToString();
        
        var userResponse = UserResponseMapper.ToResponse(user, profileType, profile?.Id);
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken(user.Id);
        await refreshTokenRepository.SaveAsync(
            user.Id,
            refreshToken.Jti,
            refreshToken.Expiration
        );

        var response = new AuthenticationResponse(
            userResponse,
            accessToken,
            refreshToken.Token,
            1800
        );
        return response;
    }
}

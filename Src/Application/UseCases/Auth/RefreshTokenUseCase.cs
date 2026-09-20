using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.Dtos.Responses.Auth;
using Backend.Src.Application.Mappers.Auth;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Auth;

public class RefreshTokenUseCase(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtPort jwtService,
    IProfileRepository profileRepository
)
{
    public async Task<AuthenticationResponse> ExecuteAsync(RefreshTokenRequest request)
    {
        var tokenData = jwtService.ValidateRefreshToken(request.RefreshToken)
            ?? throw new InvalidRefreshTokenException();
        var exists = await refreshTokenRepository.ExistsAsync(tokenData.Jti);
        if (!exists) throw new InvalidRefreshTokenException();
        var user = await userRepository.GetByIdAsync(tokenData.UserId)
            ?? throw new InvalidRefreshTokenException();

        // Rotate refresh token
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken(user.Id);
        await refreshTokenRepository.SaveAsync(
            user.Id,
            refreshToken.Jti,
            refreshToken.Expiration
        );
        await refreshTokenRepository.DeleteAsync(tokenData.Jti);

        var profile = await profileRepository.GetByUserIdAsync(user.Id);
        string? profileType = null;
        Guid? profileId = profile?.Id;

        if (profile?.CandidateProfile is not null)
            profileType = ProfileType.Candidate.ToString();
        else if (profile?.CompanyProfile is not null)
            profileType = ProfileType.Company.ToString();

        var userResponse = UserResponseMapper.ToResponse(user, profileType, profileId);

        return new AuthenticationResponse(
            userResponse,
            accessToken,
            refreshToken.Token,
            1800
        );
    }
}
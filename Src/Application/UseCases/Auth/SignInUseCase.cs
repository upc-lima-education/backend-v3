using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.Dtos.Responses.Auth;
using Backend.Src.Application.Mappers.Auth;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Auth;

public class SignInUseCase(
    IUserRepository userRepository,
    IPasswordHashPort passwordHashService,
    IJwtPort jwtService,
    IRefreshTokenRepository refreshTokenRepository,
    IProfileRepository profileRepository
)
{
    public async Task<AuthenticationResponse> ExecuteAsync(SignInRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);

        if (user is null || string.IsNullOrEmpty(user.Password))
            throw new InvalidCredentialsException();
        if (!passwordHashService.VerifyPassword(request.Password, user.Password))
            throw new InvalidCredentialsException();

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken(user.Id);
        await refreshTokenRepository.SaveAsync(
            user.Id,
            refreshToken.Jti,
            refreshToken.Expiration
        );

        var profile = await profileRepository.GetByUserIdAsync(user.Id);
        string? profileType = null;
        Guid? profileId = profile?.Id;

        if (profile?.CandidateProfile is not null)
            profileType = ProfileType.Candidate.ToString();
        else if (profile?.CompanyProfile is not null)
            profileType = ProfileType.Company.ToString();

        var userResponse = UserResponseMapper.ToResponse(user, profileType, profileId);
        var response = new AuthenticationResponse(
            userResponse,
            accessToken,
            refreshToken.Token,
            1800
        );
        return response;
    }
}
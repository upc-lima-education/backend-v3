using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Dtos.Responses.Auth;
using Backend.Src.Application.Mappers.Auth;
using Backend.Src.Application.UseCases.Profiles;
using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Auth;

public class SignUpUseCase(
    IUserRepository userRepository,
    IPasswordHashPort passwordHashService,
    IJwtPort jwtService,
    IRefreshTokenRepository refreshTokenRepository,
    IValidator<SignUpRequest> validator,
    ExternalCreateProfileUseCase bootstrapProfileUseCase
)
{
    public async Task<AuthenticationResponse> ExecuteAsync(SignUpRequest request)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null) throw new EmailAlreadyExistsException(request.Email);

        var passwordHash = passwordHashService.HashPassword(request.Password);

        var user = new User(
            request.Email,
            false, //IsEmailVerified
            passwordHash
        );

        await userRepository.CreateAsync(user);

        var effectiveProfileType = request.ProfileType ?? ProfileType.Candidate;
        var profile = await bootstrapProfileUseCase.ExecuteAsync(
            new ExternalCreateProfileRequest(
                user.Id,
                effectiveProfileType,
                request.FirstName ?? string.Empty,
                request.LastName ?? string.Empty,
                string.Empty
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
        return new AuthenticationResponse(
            userResponse,
            accessToken,
            refreshToken.Token,
            1800
        );
    }
}

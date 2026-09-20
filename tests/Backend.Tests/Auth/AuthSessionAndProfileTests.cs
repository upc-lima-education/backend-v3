using Backend.Src.Application.Dtos.Data.Auth;
using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.UseCases.Auth;
using Backend.Src.Application.UseCases.Profiles;
using Backend.Src.Domain.Contracts.Auth;
using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Domain.Repositories.Profiles;
using NSubstitute;
using Xunit;

namespace Backend.Tests.Auth;

public class AuthSessionAndProfileTests
{
    [Fact]
    public async Task SignInUseCase_WhenUserHasCandidateProfile_ReturnsCandidateProfileTypeAndProfileId()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var hashPort = Substitute.For<IPasswordHashPort>();
        var jwtPort = Substitute.For<IJwtPort>();
        var tokenRepo = Substitute.For<IRefreshTokenRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();

        var user = new User("user@test.com", true, "hashed_pwd");
        var profile = new Profile(user.Id, "Candidate profile", "150101", null, "999888777", []);
        var candidate = new CandidateProfile(profile.Id, profile, "Ana", "Lopez", null);
        profile.CandidateProfile = candidate;

        userRepo.GetByEmailAsync("user@test.com").Returns(user);
        hashPort.VerifyPassword("plain_pwd", "hashed_pwd").Returns(true);
        jwtPort.GenerateAccessToken(user).Returns("access_token_xyz");
        jwtPort.GenerateRefreshToken(user.Id).Returns(new GeneratedRefreshToken("ref_token", "jti_123", TimeSpan.FromDays(7)));
        profileRepo.GetByUserIdAsync(user.Id).Returns(profile);

        var useCase = new SignInUseCase(userRepo, hashPort, jwtPort, tokenRepo, profileRepo);

        // Act
        var result = await useCase.ExecuteAsync(new SignInRequest("user@test.com", "plain_pwd"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Candidate", result.User.ProfileType);
        Assert.Equal(profile.Id, result.User.ProfileId);
        Assert.Equal("access_token_xyz", result.AccessToken);
    }

    [Fact]
    public async Task GetCurrentUserUseCase_WhenUserHasCompanyProfile_ReturnsCompanyProfileTypeAndProfileId()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();

        var user = new User("company@test.com", true, "hashed_pwd");
        var profile = new Profile(user.Id, "Company profile", "150101", null, "999888777", []);
        var company = new CompanyProfile(profile.Id, profile, "Tech Inc", "Tech", "20123456789", null, null);
        profile.CompanyProfile = company;

        userRepo.GetByIdAsync(user.Id).Returns(user);
        profileRepo.GetByUserIdAsync(user.Id).Returns(profile);

        var useCase = new GetCurrentUserUseCase(userRepo, profileRepo);

        // Act
        var result = await useCase.ExecuteAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Company", result.ProfileType);
        Assert.Equal(profile.Id, result.ProfileId);
    }

    [Fact]
    public async Task GetMyProfileUseCase_WhenProfileExists_ReturnsProfileResponse()
    {
        // Arrange
        var profileRepo = Substitute.For<IProfileRepository>();
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Mi perfil", "150101", null, "999888777", []);
        var candidate = new CandidateProfile(profile.Id, profile, "Maria", "Perez", null);
        profile.CandidateProfile = candidate;

        profileRepo.GetByUserIdAsync(userId).Returns(profile);

        var useCase = new GetMyProfileUseCase(profileRepo);

        // Act
        var result = await useCase.ExecuteAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(profile.Id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Maria", result.Candidate?.FirstName);
    }

    [Fact]
    public async Task VerifyCompanyProfileUseCase_WhenOwnerAttemptsSelfVerification_ThrowsSelfCompanyVerificationForbiddenException()
    {
        // Arrange
        var profileRepo = Substitute.For<IProfileRepository>();
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Company profile", "150101", null, "999888777", []);
        var company = new CompanyProfile(profile.Id, profile, "My Company SAC", "Tech", "20123456789", null, null);
        profile.CompanyProfile = company;

        profileRepo.GetByIdForUpdateAsync(profile.Id).Returns(profile);

        var useCase = new VerifyCompanyProfileUseCase(profileRepo);

        // Act & Assert
        await Assert.ThrowsAsync<SelfCompanyVerificationForbiddenException>(() => useCase.ExecuteAsync(profile.Id, userId));
    }

    [Fact]
    public async Task SignUpUseCase_WhenCandidateSignUp_BootstrapsProfileAndReturnsProfileIdAndType()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var hashPort = Substitute.For<IPasswordHashPort>();
        var jwtPort = Substitute.For<IJwtPort>();
        var tokenRepo = Substitute.For<IRefreshTokenRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();
        var validator = Substitute.For<FluentValidation.IValidator<SignUpRequest>>();
        validator.ValidateAsync(Arg.Any<SignUpRequest>(), Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        userRepo.GetByEmailAsync("newcandidate@test.com").Returns((User?)null);
        hashPort.HashPassword("Password123!").Returns("hashed_pwd");
        jwtPort.GenerateAccessToken(Arg.Any<User>()).Returns("access_token_123");
        jwtPort.GenerateRefreshToken(Arg.Any<Guid>()).Returns(new GeneratedRefreshToken("ref_token", "jti_123", TimeSpan.FromDays(7)));

        var bootstrapProfile = new ExternalCreateProfileUseCase(profileRepo);
        var useCase = new SignUpUseCase(userRepo, hashPort, jwtPort, tokenRepo, validator, bootstrapProfile);

        // Act
        var result = await useCase.ExecuteAsync(new SignUpRequest("newcandidate@test.com", "Password123!", Backend.Src.Application.Dtos.Enums.Profiles.ProfileType.Candidate));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Candidate", result.User.ProfileType);
        Assert.NotNull(result.User.ProfileId);
        Assert.NotEqual(Guid.Empty, result.User.ProfileId);
        await profileRepo.Received(1).CreateAsync(Arg.Is<Profile>(p => p.CandidateProfile != null));
    }

    [Fact]
    public async Task SignUpUseCase_WhenCompanySignUp_BootstrapsCompanyProfileAndReturnsProfileIdAndType()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var hashPort = Substitute.For<IPasswordHashPort>();
        var jwtPort = Substitute.For<IJwtPort>();
        var tokenRepo = Substitute.For<IRefreshTokenRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();
        var validator = Substitute.For<FluentValidation.IValidator<SignUpRequest>>();
        validator.ValidateAsync(Arg.Any<SignUpRequest>(), Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        userRepo.GetByEmailAsync("company@test.com").Returns((User?)null);
        hashPort.HashPassword("Password123!").Returns("hashed_pwd");
        jwtPort.GenerateAccessToken(Arg.Any<User>()).Returns("access_token_123");
        jwtPort.GenerateRefreshToken(Arg.Any<Guid>()).Returns(new GeneratedRefreshToken("ref_token", "jti_123", TimeSpan.FromDays(7)));

        var bootstrapProfile = new ExternalCreateProfileUseCase(profileRepo);
        var useCase = new SignUpUseCase(userRepo, hashPort, jwtPort, tokenRepo, validator, bootstrapProfile);

        // Act
        var result = await useCase.ExecuteAsync(new SignUpRequest("company@test.com", "Password123!", Backend.Src.Application.Dtos.Enums.Profiles.ProfileType.Company));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Company", result.User.ProfileType);
        Assert.NotNull(result.User.ProfileId);
        Assert.NotEqual(Guid.Empty, result.User.ProfileId);
        await profileRepo.Received(1).CreateAsync(Arg.Is<Profile>(p => p.CompanyProfile != null));
    }
}

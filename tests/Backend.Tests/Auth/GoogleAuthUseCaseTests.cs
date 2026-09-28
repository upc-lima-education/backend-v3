using System.Text.Json;
using Backend.Src.Api.Rest.Controllers.Profiles;
using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Mappers.Auth;
using Backend.Src.Application.UseCases.Profiles;
using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Infrastructure.Contracts.Auth.OAuth;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Backend.Tests.Auth;

public class GoogleAuthUseCaseTests
{
    [Fact]
    public async Task ExternalCreateProfileUseCase_WhenProfileTypeIsCompany_CreatesCompanyProfileWithValidName()
    {
        // Arrange
        var profileRepository = Substitute.For<IProfileRepository>();
        var useCase = new CreateProfileUseCase(profileRepository);

        var request = new CreateProfileRequest(Guid.NewGuid(), ProfileType.Company);

        // Act
        var createdProfile = await useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(createdProfile);
        Assert.NotNull(createdProfile.CompanyProfile);
        Assert.Null(createdProfile.CandidateProfile);
        Assert.Equal("Tech Corp", createdProfile.CompanyProfile.CompanyName);
        Assert.Equal("https://lh3.googleusercontent.com/company.png", createdProfile.ProfilePicture);
        await profileRepository.Received(1).CreateAsync(Arg.Any<Profile>());
    }

    [Fact]
    public void UserResponseMapper_ToResponse_IncludesProfileIdAndProfileType()
    {
        // Arrange
        var user = new User("user@gmail.com", true, null);
        var profileId = Guid.NewGuid();

        // Act
        var response = UserResponseMapper.ToData(user, ProfileType.Candidate, profileId);

        // Assert
        Assert.Equal(user.Id, response.Id);
        Assert.Equal("user@gmail.com", response.Email);
        Assert.Equal(ProfileType.Candidate, response.ProfileType);
        Assert.Equal(profileId, response.ProfileId);
    }

    [Fact]
    public void GoogleIdTokenResponse_Deserialization_HandlesStringAndBooleanEmailVerified()
    {
        // Arrange: Google sends email_verified as a string "true" and exp as string or number
        var json = """
        {
            "aud": "client-id.apps.googleusercontent.com",
            "exp": "1726000000",
            "iss": "https://accounts.google.com",
            "sub": "123456789",
            "email": "juan@gmail.com",
            "email_verified": "true",
            "name": "Juan Perez",
            "given_name": "Juan",
            "family_name": "Perez",
            "picture": "https://lh3.googleusercontent.com/photo.jpg"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GoogleIdTokenResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("juan@gmail.com", response.Email);
        Assert.True(response.IsEmailVerified);
        Assert.Equal(1726000000L, response.ExpiresAtUnix);
        Assert.Equal("Juan", response.FirstName);
    }

    [Fact]
    public async Task ProfilePictureController_WhenPictureIsExternalUrl_ReturnsRedirect()
    {
        // Arrange
        var profileRepository = Substitute.For<IProfileRepository>();
        var fileStorage = Substitute.For<IFileStoragePort>();

        var userId = Guid.NewGuid();
        var googleAvatarUrl = "https://lh3.googleusercontent.com/a/random-avatar";
        var profile = new Profile(userId, googleAvatarUrl);

        profileRepository.GetByUserIdAsync(userId).Returns(profile);

        var controller = new ProfilePictureController(profileRepository, fileStorage);

        // Act
        var result = await controller.Get(userId);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal(googleAvatarUrl, redirectResult.Url);
        await fileStorage.DidNotReceive().DownloadAsync(Arg.Any<string>());
    }
}

using Backend.Src.Application.UseCases.Curriculums;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Curriculums;
using NSubstitute;
using Xunit;

namespace Backend.Tests.Curriculums;

public class CvProcessingStatusTests
{
    [Fact]
    public void Cv_TracksProcessingLifecycleAndClearsPreviousError()
    {
        var cv = new Cv(Guid.NewGuid(), "CV de prueba", false);

        cv.MarkProcessing();
        Assert.Equal(CvProcessingStatus.Processing, cv.ProcessingStatus);
        Assert.Null(cv.ProcessingError);

        cv.MarkFailed("falló");
        Assert.Equal(CvProcessingStatus.Failed, cv.ProcessingStatus);
        Assert.Equal("falló", cv.ProcessingError);

        cv.MarkReady();
        Assert.Equal(CvProcessingStatus.Ready, cv.ProcessingStatus);
        Assert.Null(cv.ProcessingError);
    }

    [Fact]
    public async Task GetCvProcessingStatusUseCase_ReturnsOwnedCvState()
    {
        var cvRepository = Substitute.For<ICvRepository>();
        var profileRepository = Substitute.For<IProfileRepository>();
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Candidate", "150101", null, "999888777", []);
        profile.CandidateProfile = new CandidateProfile(profile.Id, profile, "Ana", "Torres", null);
        var cv = new Cv(profile.Id, "CV Ana", false);
        cv.MarkProcessing();

        profileRepository.GetByUserIdAsync(userId).Returns(profile);
        cvRepository.GetByIdAsync(cv.Id).Returns(cv);

        var useCase = new GetCvProcessingStatusUseCase(cvRepository, profileRepository);
        var result = await useCase.ExecuteAsync(cv.Id, userId);

        Assert.Equal(cv.Id, result.Id);
        Assert.Equal("Processing", result.Status);
        Assert.Null(result.Error);
    }
}

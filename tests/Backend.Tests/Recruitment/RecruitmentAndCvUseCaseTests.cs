using Backend.Src.Application.UseCases.Curriculums;
using Backend.Src.Application.UseCases.Recruitment;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Entities.Jobs;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Entities.Recruitment;
using Backend.Src.Domain.Entities.Skills;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Repositories.Recruitment;
using Backend.Src.Domain.ValueObjects.Jobs;
using NSubstitute;
using Xunit;

namespace Backend.Tests.Recruitment;

public class RecruitmentAndCvUseCaseTests
{
    [Fact]
    public async Task GetMyJobApplicationsUseCase_ReturnsApplicationsWithJobDetails()
    {
        // Arrange
        var appRepo = Substitute.For<IJobApplicationRepository>();
        var jobRepo = Substitute.For<IJobRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();

        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Candidate", "150101", null, "999888777", []);
        var candidate = new CandidateProfile(profile.Id, profile, "Lucia", "Ramos", null);
        profile.CandidateProfile = candidate;

        var jobId = Guid.NewGuid();
        var job = new Job(
            Guid.NewGuid(),
            "Senior Backend Engineer",
            "Job description",
            JobType.Remote,
            WorkHours.FullTime,
            [new Skill("C#")],
            Experience.TwoOrMoreYears,
            EducationLevel.University,
            "150101",
            "Av. Principal 123",
            5000,
            8000,
            Currency.PEN,
            SalaryPeriod.Monthly,
            CompensationType.Fixed,
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(20),
            OriginPage.Internal,
            "Acme Tech SAC",
            null,
            null
        );

        var application = new JobApplication(jobId, profile.Id, "key/cv.pdf");

        profileRepo.GetByUserIdAsync(userId).Returns(profile);
        appRepo.GetByCandidateIdAsync(profile.Id).Returns([application]);
        jobRepo.GetByIdAsync(jobId).Returns(job);

        var useCase = new GetMyJobApplicationsUseCase(appRepo, jobRepo, profileRepo);

        // Act
        var result = await useCase.ExecuteAsync(userId);

        // Assert
        Assert.Single(result);
        Assert.Equal(application.Id, result[0].Id);
        Assert.Equal(jobId, result[0].JobId);
        Assert.Equal("Senior Backend Engineer", result[0].JobTitle);
        Assert.Equal("Acme Tech SAC", result[0].CompanyName);
        Assert.Equal("Pending", result[0].Status);
    }

    [Fact]
    public async Task GetJobApplicationsByJobUseCase_ReturnsApplicationsWithCandidateSummary()
    {
        // Arrange
        var appRepo = Substitute.For<IJobApplicationRepository>();
        var jobRepo = Substitute.For<IJobRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();

        var companyUserId = Guid.NewGuid();
        var companyProfile = new Profile(companyUserId, "Company", "150101", null, "999111222", []);
        var company = new CompanyProfile(companyProfile.Id, companyProfile, "Dev Corp", "Tech", "20123456789", null, null);
        companyProfile.CompanyProfile = company;

        var jobId = Guid.NewGuid();
        var job = new Job(
            companyProfile.Id,
            "Frontend Engineer",
            "Job description",
            JobType.Remote,
            WorkHours.FullTime,
            [],
            Experience.NoExperienceNeeded,
            EducationLevel.University,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            DateTime.UtcNow.AddDays(-5),
            DateTime.UtcNow.AddDays(15),
            OriginPage.Internal,
            null,
            null,
            null
        );

        var candidateUserId = Guid.NewGuid();
        var candidateProfile = new Profile(candidateUserId, "Candidate", "150101", "avatar.jpg", "999444333", [new Skill("React")]);
        var candidate = new CandidateProfile(candidateProfile.Id, candidateProfile, "Sofia", "Vargas", null);
        candidateProfile.CandidateProfile = candidate;

        var application = new JobApplication(jobId, candidateProfile.Id, "key/cv.pdf");

        jobRepo.GetByIdAsync(jobId).Returns(job);
        profileRepo.GetByUserIdAsync(companyUserId).Returns(companyProfile);
        appRepo.GetByJobIdAsync(jobId).Returns([application]);
        profileRepo.GetByIdAsync(candidateProfile.Id).Returns(candidateProfile);

        var useCase = new GetJobApplicationsByJobUseCase(appRepo, jobRepo, profileRepo);

        // Act
        var result = await useCase.ExecuteAsync(jobId, companyUserId);

        // Assert
        Assert.Single(result);
        Assert.Equal(application.Id, result[0].Id);
        Assert.NotNull(result[0].Candidate);
        Assert.Equal("Sofia", result[0].Candidate!.FirstName);
        Assert.Equal("Vargas", result[0].Candidate!.LastName);
        Assert.Equal("avatar.jpg", result[0].Candidate!.ProfilePicture);
        Assert.Equal("999444333", result[0].Candidate!.PhoneNumber);
        Assert.Equal(["React"], result[0].Candidate!.Skills);
    }

    [Fact]
    public async Task GetMyCvsUseCase_ReturnsCandidateCvs()
    {
        // Arrange
        var cvRepo = Substitute.For<ICvRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();

        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Candidate", "150101", null, "999888777", []);
        var candidate = new CandidateProfile(profile.Id, profile, "Pedro", "Gomez", null);
        profile.CandidateProfile = candidate;

        var cv1 = new Cv(profile.Id, "CV Desarrollador .NET", true);
        cv1.SetFileContent("cvs/pedro.pdf");
        var cv2 = new Cv(profile.Id, "CV Fullstack", false);

        profileRepo.GetByUserIdAsync(userId).Returns(profile);
        cvRepo.GetAllByCandidateIdAsync(profile.Id).Returns([cv1, cv2]);

        var useCase = new GetMyCvsUseCase(cvRepo, profileRepo);

        // Act
        var result = await useCase.ExecuteAsync(userId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("CV Desarrollador .NET", result[0].Title);
        Assert.True(result[0].IsCurrent);
        Assert.True(result[0].HasFileContent);
        Assert.Equal("CV Fullstack", result[1].Title);
        Assert.False(result[1].IsCurrent);
    }
}

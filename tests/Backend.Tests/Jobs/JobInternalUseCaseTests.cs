using Backend.Src.Application.Dtos.Data.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using Backend.Src.Application.Mappers.Jobs;
using Backend.Src.Domain.Entities.Jobs;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Entities.Skills;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Jobs;
using NSubstitute;
using Xunit;

namespace Backend.Tests.Jobs;

public class JobInternalUseCaseTests
{
    [Fact]
    public void InternalJob_WhenCreatedWithCompanyProfile_HasCompanyImageAndNonEmptyCompanyResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new Profile(
            userId,
            "Empresa de tecnología líder",
            "150101",
            "profiles/company-logo.png",
            "999111222",
            [new Skill("C#"), new Skill(".NET")]
        );

        var companyProfile = new CompanyProfile(
            profile.Id,
            profile,
            "Tech Solutions SAC",
            "Tecnología",
            "20601234567",
            "https://techsolutions.pe",
            "50-100"
        );
        profile.CompanyProfile = companyProfile;

        var request = new CreateInternalJobRequest(
            "Backend Senior Developer",
            "Construcción de APIs con .NET Core",
            JobType.Remote,
            WorkHours.FullTime,
            ["C#", ".NET"],
            Experience.TwoOrMoreYears,
            EducationLevel.University,
            new JobLocationData("150101", "Av. Javier Prado 1234"),
            new JobPaymentData(8000, 10000, Currency.PEN, SalaryPeriod.Monthly, CompensationType.Fixed),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            "https://techsolutions.pe/apply"
        );

        var skills = new List<Skill> { new("C#"), new(".NET") };

        // Act
        var job = JobMapper.ToEntity(
            request,
            profile.Id,
            skills,
            profile.CompanyProfile.CompanyName,
            profile.ProfilePicture
        );
        job.AssociateCompany(profile.CompanyProfile);

        var response = JobMapper.ToResponse(job);
        var listItemResponse = JobListItemResponseMapper.ToResponse(job);

        // Assert
        Assert.NotNull(response.Company);
        Assert.Equal(profile.Id, response.Company.Id);
        Assert.Equal("Tech Solutions SAC", response.Company.Name);
        Assert.Equal("profiles/company-logo.png", response.Company.ImageUrl);

        Assert.Equal("Tech Solutions SAC", listItemResponse.CompanyName);
        Assert.Equal("profiles/company-logo.png", listItemResponse.CompanyImage);
        Assert.Equal("Backend Senior Developer", listItemResponse.Title);
    }

    [Fact]
    public void JobListItemResponseMapper_WhenCompanyProfileIsNull_UsesExternalCompanyImageFallback()
    {
        // Arrange
        var job = new Job(
            Guid.NewGuid(),
            "DevOps Engineer",
            "Cloud infrastructure",
            JobType.Remote,
            WorkHours.FullTime,
            [new Skill("Docker"), new Skill("Kubernetes")],
            Experience.OneYear,
            EducationLevel.University,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(15),
            OriginPage.Internal,
            "Cloud Corp",
            null,
            null,
            "profiles/cloud-corp-logo.png"
        );

        // Act
        var listItem = JobListItemResponseMapper.ToResponse(job);

        // Assert
        Assert.Equal("Cloud Corp", listItem.CompanyName);
        Assert.Equal("profiles/cloud-corp-logo.png", listItem.CompanyImage);
    }

    [Fact]
    public void InternalJob_WhenCreatedWithoutProfilePicture_ReturnsNullCompanyImageForDefaultLogoFallback()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new Profile(
            userId,
            "Empresa sin logo todavía",
            "150101",
            null,
            "999111222",
            [new Skill("C#")]
        );

        var companyProfile = new CompanyProfile(
            profile.Id,
            profile,
            "Startup Nova",
            "Tecnología",
            "20609876543",
            "https://novastartup.pe",
            "1-10"
        );
        profile.CompanyProfile = companyProfile;

        var request = new CreateInternalJobRequest(
            "Junior QA Engineer",
            "Testing de software",
            JobType.InPerson,
            WorkHours.FullTime,
            ["C#"],
            Experience.NoExperienceNeeded,
            EducationLevel.Technical,
            new JobLocationData("150101", "Av. Brasil 500"),
            null,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            null
        );

        var job = JobMapper.ToEntity(
            request,
            profile.Id,
            [new Skill("C#")],
            profile.CompanyProfile.CompanyName,
            profile.ProfilePicture
        );
        job.AssociateCompany(profile.CompanyProfile);

        // Act
        var response = JobMapper.ToResponse(job);
        var listItemResponse = JobListItemResponseMapper.ToResponse(job);

        // Assert
        Assert.NotNull(response.Company);
        Assert.Equal("Startup Nova", response.Company.Name);
        Assert.Null(response.Company.ImageUrl);
        Assert.Null(listItemResponse.CompanyImage);
    }

    [Fact]
    public async Task CreateInternalJobUseCase_WhenExecuted_DoesNotAttachNavigationBeforeSavingAndReturnsCompanyResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new Profile(
            userId,
            "Empresa de tecnología líder",
            "150101",
            "profiles/company-logo.png",
            "999111222",
            [new Skill("C#"), new Skill(".NET")]
        );

        var companyProfile = new CompanyProfile(
            profile.Id,
            profile,
            "Tech Solutions SAC",
            "Tecnología",
            "20601234567",
            "https://techsolutions.pe",
            "50-100"
        );
        profile.CompanyProfile = companyProfile;

        var request = new CreateInternalJobRequest(
            "Backend Senior Developer",
            "Construcción de APIs con .NET Core y arquitecturas limpias para sistemas distribuidos",
            JobType.Remote,
            WorkHours.FullTime,
            ["c#", ".net"],
            Experience.TwoOrMoreYears,
            EducationLevel.University,
            new JobLocationData("150101", "Av. Javier Prado 1234"),
            new JobPaymentData(8000, 10000, Currency.PEN, SalaryPeriod.Monthly, CompensationType.Fixed),
            DateTime.UtcNow.AddHours(-1), // Publicación abierta hace 1 hora (debe ser válida con la nueva tolerancia)
            DateTime.UtcNow.AddDays(30),
            "https://techsolutions.pe/apply"
        );

        var jobRepository = NSubstitute.Substitute.For<IJobRepository>();
        var profileRepository = NSubstitute.Substitute.For<IProfileRepository>();
        var skillRepository = NSubstitute.Substitute.For<Backend.Src.Domain.Repositories.Skills.ISkillRepository>();

        profileRepository.GetByUserIdAsync(userId).Returns(profile);
        skillRepository.GetSkillListByNameList(NSubstitute.Arg.Any<List<string>>())
            .Returns(call => Task.FromResult<IReadOnlyList<Skill>>((call.Arg<List<string>>()).Select(s => new Skill(s)).ToList()));

        var skillResolver = new Backend.Src.Application.Resolvers.Skills.SkillResolver(skillRepository);
        var validator = new Backend.Src.Application.Validators.Jobs.CreateInternalJobValidator();

        Job? capturedJobAtSave = null;
        jobRepository.CreateAsync(NSubstitute.Arg.Do<Job>(j =>
        {
            // Crucial: EF Core no debe recibir la entidad detached CompanyProfile en j.Company
            // al ejecutar CreateAsync, previniendo violación de PK/UK en inserción.
            capturedJobAtSave = j;
            Assert.Null(j.Company);
            Assert.Equal(profile.Id, j.CompanyId);
        })).Returns(Task.CompletedTask);

        var useCase = new Backend.Src.Application.UseCases.Jobs.CreateInternalJobUseCase(
            jobRepository,
            profileRepository,
            skillResolver,
            validator
        );

        // Act
        var response = await useCase.ExecuteAsync(request, userId);

        // Assert
        Assert.NotNull(capturedJobAtSave);
        Assert.NotNull(response.Company);
        Assert.Equal(profile.Id, response.Company.Id);
        Assert.Equal("Tech Solutions SAC", response.Company.Name);
        Assert.Equal("profiles/company-logo.png", response.Company.ImageUrl);
        Assert.Equal("Backend Senior Developer", response.Title);
        await jobRepository.Received(1).CreateAsync(NSubstitute.Arg.Any<Job>());
    }

    [Fact]
    public async Task CreateInternalJobValidator_WhenOpensAtIsOneHourAgo_PassesValidation()
    {
        // Arrange
        var validator = new Backend.Src.Application.Validators.Jobs.CreateInternalJobValidator();
        var request = new CreateInternalJobRequest(
            "Software Architect",
            "Diseño y evolución de arquitecturas distribuidas de alta concurrencia.",
            JobType.Remote,
            WorkHours.FullTime,
            ["Architecture"],
            Experience.TwoOrMoreYears,
            EducationLevel.University,
            new JobLocationData("150101", "Calle Las Begonias 450"),
            null,
            DateTime.UtcNow.AddHours(-2), // 2 horas en el pasado
            DateTime.UtcNow.AddDays(30),
            null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
}

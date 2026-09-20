using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Src.Application.Dtos.Data.Profiles;
using Backend.Src.Application.Dtos.Responses.Profiles;
using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Application.UseCases.Profiles;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Entities.Skills;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Profiles;
using NSubstitute;
using Xunit;

namespace Backend.Tests.Profiles;

public class ProfileMappingAndUseCaseTests
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    [Fact]
    public void ProfileMapper_ToResponse_WhenCandidateHasFullData_MapsAllCollectionsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new Profile(
            userId,
            "Desarrollador Full Stack",
            "150101",
            "profiles/avatar.png",
            "999888777",
            [new Skill("C#"), new Skill("React")]
        );

        var candidate = new CandidateProfile(
            profile.Id,
            profile,
            "Mariana",
            "Ana",
            "12345678"
        );

        var languages = new List<LanguageKnown>
        {
            new(profile.Id, LanguageCode.Es, LanguageLevel.C2),
            new(profile.Id, LanguageCode.En, LanguageLevel.B2)
        };
        candidate.UpdateLanguages(languages);

        var educations = new List<Education>
        {
            new(profile.Id, "Universidad Nacional", "Ingeniería", "Sistemas", new DateOnly(2018, 3, 1), new DateOnly(2023, 12, 15))
        };
        candidate.UpdateEducations(educations);

        var workExperiences = new List<WorkExperience>
        {
            new(profile.Id, "Tech Corp", "Software Engineer", "Desarrollo backend", new DateOnly(2024, 1, 1), null)
        };
        candidate.UpdateWorkExperiences(workExperiences);

        profile.CandidateProfile = candidate;

        // Act
        var response = ProfileMapper.ToResponse(profile);

        // Assert
        Assert.Equal(profile.Id, response.Id);
        Assert.Equal(userId, response.UserId);
        Assert.Equal("Desarrollador Full Stack", response.Description);
        Assert.Equal(["C#", "React"], response.Skills);
        Assert.Equal("profiles/avatar.png", response.ProfilePicture);
        Assert.Equal("999888777", response.PhoneNumber);
        Assert.NotNull(response.Candidate);
        Assert.Equal("Mariana", response.Candidate.FirstName);
        Assert.Equal("Ana", response.Candidate.LastName);
        Assert.Null(response.Company);

        // Verify Languages
        Assert.NotNull(response.Languages);
        Assert.Equal(2, response.Languages.Count);
        Assert.Equal(LanguageCode.Es, response.Languages[0].Code);
        Assert.Equal(LanguageLevel.C2, response.Languages[0].Level);
        Assert.Equal(LanguageCode.En, response.Languages[1].Code);
        Assert.Equal(LanguageLevel.B2, response.Languages[1].Level);

        // Verify Educations
        Assert.NotNull(response.Educations);
        Assert.Single(response.Educations);
        Assert.Equal("Universidad Nacional", response.Educations[0].Institution);
        Assert.Equal("Ingeniería", response.Educations[0].Degree);
        Assert.Equal("Sistemas", response.Educations[0].FieldOfStudy);
        Assert.Equal(new DateOnly(2018, 3, 1), response.Educations[0].StartDate);
        Assert.Equal(new DateOnly(2023, 12, 15), response.Educations[0].EndDate);

        // Verify WorkExperiences
        Assert.NotNull(response.WorkExperiences);
        Assert.Single(response.WorkExperiences);
        Assert.Equal("Tech Corp", response.WorkExperiences[0].Company);
        Assert.Equal("Software Engineer", response.WorkExperiences[0].Position);
        Assert.Equal("Desarrollo backend", response.WorkExperiences[0].Description);
        Assert.Equal(new DateOnly(2024, 1, 1), response.WorkExperiences[0].StartDate);
        Assert.Null(response.WorkExperiences[0].EndDate);

        Assert.True(response.IsComplete);
    }

    [Fact]
    public void ProfileMapper_ToResponse_WhenCompanyProfile_ReturnsEmptyListsNeverNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new Profile(
            userId,
            "Empresa de Tecnología",
            "150101",
            null,
            "999111222",
            null
        );

        var company = new CompanyProfile(
            profile.Id,
            profile,
            "Tech Solutions SAC",
            "Tecnología",
            "20123456789",
            "https://techsolutions.pe",
            "11-50"
        );
        profile.CompanyProfile = company;

        // Act
        var response = ProfileMapper.ToResponse(profile);

        // Assert
        Assert.Equal(profile.Id, response.Id);
        Assert.Equal(userId, response.UserId);
        Assert.Null(response.Candidate);
        Assert.NotNull(response.Company);
        Assert.Equal("Tech Solutions SAC", response.Company.CompanyName);
        Assert.Equal("Tecnología", response.Company.Sector);
        Assert.Equal("20123456789", response.Company.Ruc);

        // Ensure empty lists, never null
        Assert.NotNull(response.Languages);
        Assert.Empty(response.Languages);
        Assert.NotNull(response.Educations);
        Assert.Empty(response.Educations);
        Assert.NotNull(response.WorkExperiences);
        Assert.Empty(response.WorkExperiences);
    }

    [Fact]
    public void ProfileMapper_ToResponse_WhenCandidateHasNoCollections_ReturnsEmptyLists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new Profile(
            userId,
            "Junior Dev",
            "150101",
            null,
            "999000111",
            []
        );

        var candidate = new CandidateProfile(
            profile.Id,
            profile,
            "Carlos",
            "Gomez",
            "87654321"
        );
        profile.CandidateProfile = candidate;

        // Act
        var response = ProfileMapper.ToResponse(profile);

        // Assert
        Assert.NotNull(response.Languages);
        Assert.Empty(response.Languages);
        Assert.NotNull(response.Educations);
        Assert.Empty(response.Educations);
        Assert.NotNull(response.WorkExperiences);
        Assert.Empty(response.WorkExperiences);
    }

    [Fact]
    public async Task GetProfileByProfileIdUseCase_WhenProfileExists_ReturnsProfileResponse()
    {
        // Arrange
        var profileRepository = Substitute.For<IProfileRepository>();
        var profileId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var profile = new Profile(
            userId,
            "Perfil Profesional",
            "150101",
            null,
            "999999999",
            [new Skill("Excel"), new Skill("Atención al cliente")]
        );

        var candidate = new CandidateProfile(
            profileId,
            profile,
            "Mariana",
            "Ana",
            null
        );

        candidate.UpdateLanguages([new LanguageKnown(profileId, LanguageCode.Es, LanguageLevel.C2)]);
        candidate.UpdateEducations([new Education(profileId, "Universidad X", "Ingeniería", "Sistemas", new DateOnly(2020, 3, 1), null)]);
        candidate.UpdateWorkExperiences([new WorkExperience(profileId, "Empresa X", "Asistente", "Atención al cliente", new DateOnly(2024, 1, 1), null)]);
        profile.CandidateProfile = candidate;

        profileRepository.GetByIdAsync(profileId).Returns(profile);

        var useCase = new GetProfileByProfileIdUseCase(profileRepository);

        // Act
        var result = await useCase.ExecuteAsync(profileId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(profile.Id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Mariana", result.Candidate?.FirstName);
        Assert.Equal("Ana", result.Candidate?.LastName);
        Assert.Single(result.Languages);
        Assert.Single(result.Educations);
        Assert.Single(result.WorkExperiences);

        await profileRepository.Received(1).GetByIdAsync(profileId);
    }

    [Fact]
    public async Task GetProfileByProfileIdUseCase_WhenNotFound_ThrowsProfileNotFoundException()
    {
        // Arrange
        var profileRepository = Substitute.For<IProfileRepository>();
        var profileId = Guid.NewGuid();
        profileRepository.GetByIdAsync(profileId).Returns((Profile?)null);

        var useCase = new GetProfileByProfileIdUseCase(profileRepository);

        // Act & Assert
        await Assert.ThrowsAsync<ProfileNotFoundException>(() => useCase.ExecuteAsync(profileId));
    }

    [Fact]
    public void IndividualMappers_ToData_MapFieldsCorrectly()
    {
        // LanguageKnownMapper
        var lang = new LanguageKnown(Guid.NewGuid(), LanguageCode.En, LanguageLevel.B2);
        var langData = LanguageKnownMapper.ToData(lang);
        Assert.Equal(LanguageCode.En, langData.Code);
        Assert.Equal(LanguageLevel.B2, langData.Level);

        // EducationMapper
        var edu = new Education(Guid.NewGuid(), "MIT", "Master", "CS", new DateOnly(2021, 9, 1), new DateOnly(2023, 6, 30));
        var eduData = EducationMapper.ToData(edu);
        Assert.Equal("MIT", eduData.Institution);
        Assert.Equal("Master", eduData.Degree);
        Assert.Equal("CS", eduData.FieldOfStudy);
        Assert.Equal(new DateOnly(2021, 9, 1), eduData.StartDate);
        Assert.Equal(new DateOnly(2023, 6, 30), eduData.EndDate);

        // WorkExperienceMapper
        var exp = new WorkExperience(Guid.NewGuid(), "Google", "Lead", "Cloud dev", new DateOnly(2022, 1, 1), null);
        var expData = WorkExperienceMapper.ToData(exp);
        Assert.Equal("Google", expData.Company);
        Assert.Equal("Lead", expData.Position);
        Assert.Equal("Cloud dev", expData.Description);
        Assert.Equal(new DateOnly(2022, 1, 1), expData.StartDate);
        Assert.Null(expData.EndDate);
    }

    [Fact]
    public void ProfileResponse_JsonSerialization_MatchesExpectedFormat()
    {
        // Arrange
        var response = new ProfileResponse(
            Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserId: Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Description: "Perfil profesional",
            Ubigeo: "150101",
            Skills: ["Excel", "Atención al cliente"],
            ProfilePicture: null,
            PhoneNumber: "999999999",
            Candidate: new CandidateProfileData("Mariana", "Ana"),
            Company: null,
            Languages: [new LanguageKnownData(LanguageCode.Es, LanguageLevel.C2)],
            Educations: [new EducationData("Universidad X", "Ingeniería", "Sistemas", new DateOnly(2020, 3, 1), null)],
            WorkExperiences: [new WorkExperienceData("Empresa X", "Asistente", "Atención al cliente", new DateOnly(2024, 1, 1), null)],
            IsComplete: true,
            CreatedAt: new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt: new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
        );

        // Act
        var json = JsonSerializer.Serialize(response, _jsonSerializerOptions);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Assert properties exist and match format
        Assert.Equal("11111111-1111-1111-1111-111111111111", root.GetProperty("id").GetString());
        Assert.Equal("22222222-2222-2222-2222-222222222222", root.GetProperty("userId").GetString());
        Assert.Equal("Perfil profesional", root.GetProperty("description").GetString());
        Assert.Equal(2, root.GetProperty("skills").GetArrayLength());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("profilePicture").ValueKind);
        Assert.Equal("999999999", root.GetProperty("phoneNumber").GetString());

        // Candidate
        var candidateElem = root.GetProperty("candidate");
        Assert.Equal("Mariana", candidateElem.GetProperty("firstName").GetString());
        Assert.Equal("Ana", candidateElem.GetProperty("lastName").GetString());

        // Company is null
        Assert.Equal(JsonValueKind.Null, root.GetProperty("company").ValueKind);

        // Languages
        var languagesElem = root.GetProperty("languages");
        Assert.Equal(1, languagesElem.GetArrayLength());
        Assert.Equal("Es", languagesElem[0].GetProperty("code").GetString());
        Assert.Equal("C2", languagesElem[0].GetProperty("level").GetString());

        // Educations
        var educationsElem = root.GetProperty("educations");
        Assert.Equal(1, educationsElem.GetArrayLength());
        Assert.Equal("Universidad X", educationsElem[0].GetProperty("institution").GetString());
        Assert.Equal("Ingeniería", educationsElem[0].GetProperty("degree").GetString());
        Assert.Equal("Sistemas", educationsElem[0].GetProperty("fieldOfStudy").GetString());
        Assert.Equal("2020-03-01", educationsElem[0].GetProperty("startDate").GetString());
        Assert.Equal(JsonValueKind.Null, educationsElem[0].GetProperty("endDate").ValueKind);

        // WorkExperiences
        var workExperiencesElem = root.GetProperty("workExperiences");
        Assert.Equal(1, workExperiencesElem.GetArrayLength());
        Assert.Equal("Empresa X", workExperiencesElem[0].GetProperty("company").GetString());
        Assert.Equal("Asistente", workExperiencesElem[0].GetProperty("position").GetString());
        Assert.Equal("Atención al cliente", workExperiencesElem[0].GetProperty("description").GetString());
        Assert.Equal("2024-01-01", workExperiencesElem[0].GetProperty("startDate").GetString());
        Assert.Equal(JsonValueKind.Null, workExperiencesElem[0].GetProperty("endDate").ValueKind);

        // Validation & Traceability
        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal("2026-08-30T00:00:00Z", root.GetProperty("createdAt").GetString());
        Assert.Equal("2026-08-30T00:00:00Z", root.GetProperty("updatedAt").GetString());
    }
}

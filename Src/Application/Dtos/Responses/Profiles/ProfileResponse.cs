using Backend.Src.Application.Dtos.Data.Profiles;

namespace Backend.Src.Application.Dtos.Responses.Profiles;

public record ProfileResponse(
    //Id
    Guid Id,
    Guid UserId,
    //Shared info
    string? Description,
    string? Ubigeo,
    List<string> Skills,
    string? ProfilePicture,
    string? PhoneNumber,
    // Candidate fields
    CandidateProfileData? Candidate,
    // Company fields
    CompanyProfileData? Company,
    // Candidate detailed lists
    List<LanguageKnownData> Languages,
    List<EducationData> Educations,
    List<WorkExperienceData> WorkExperiences,
    //Validation
    bool IsComplete,
    //Traceability
    DateTime CreatedAt,
    DateTime UpdatedAt
);

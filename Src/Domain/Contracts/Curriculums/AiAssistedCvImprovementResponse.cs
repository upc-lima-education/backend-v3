namespace Backend.Src.Domain.Contracts.Curriculums;

public record AiAssistedCvImprovementResponse(
    string? Summary,
    List<CvWorkExperienceSnapshot>? WorkExperiences,
    string? Certification,
    string? Project,
    string? Award
);
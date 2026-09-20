namespace Backend.Src.Domain.Contracts.Curriculums;

public record AiAssistedCvGenerationResponse(
    string? Headline,
    string? Description,
    List<CvWorkExperienceSnapshot> WorkExperiences,
    List<string> Skills
);

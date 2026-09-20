namespace Backend.Src.Domain.Contracts.Curriculums;

public record CvProfileSnapshot(
    string? Description,
    List<CvWorkExperienceSnapshot> WorkExperiences,
    List<string> Skills
);
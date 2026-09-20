namespace Backend.Src.Domain.Contracts.Curriculums;

public record CvWorkExperienceSnapshot(
    string Reference,
    string Position,
    string? Description
);
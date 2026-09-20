namespace Backend.Src.Domain.Contracts.Curriculums;

public record JobSnapshot(
    string Title,
    string Description,
    List<string> Skills
);
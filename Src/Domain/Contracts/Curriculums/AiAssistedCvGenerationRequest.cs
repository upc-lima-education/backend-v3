namespace Backend.Src.Domain.Contracts.Curriculums;

public record AiAssistedCvGenerationRequest(
    JobSnapshot? JobData,
    CvProfileSnapshot UserData
);
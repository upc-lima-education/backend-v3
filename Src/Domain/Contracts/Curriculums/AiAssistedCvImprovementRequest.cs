namespace Backend.Src.Domain.Contracts.Curriculums;

public record AiAssistedCvImprovementRequest(
    JobSnapshot? JobData,
    CvProfileSnapshot UserData,
    CvStructuredContentSnapshot CvData
);
using Backend.Src.Application.Dtos.Data.Curriculums;

namespace Backend.Src.Application.Dtos.Requests.Curriculums;

public record CreateCvUploadedContentRequest(
    string Title,
    bool IsCurrent,
    CvUploadedContentData Cv
);
using Backend.Src.Application.Dtos.Data.Curriculums;

namespace Backend.Src.Application.Dtos.Responses.Curriculums;

public record CvStructuredContentResponse(
    Guid Id,
    DateTime UpdatedAt,
    CvHeaderData Header,
    CvTextSectionData? Summary,
    List<CvExperienceItemData> Experiences,
    List<CvEducationItemData> Educations,
    CvTextSectionData? Skills,
    CvTextSectionData? Languages,
    CvTextSectionData? Certifications,
    CvTextSectionData? Projects,
    CvTextSectionData? Awards,
    List<CvCustomSectionData> Customs
);

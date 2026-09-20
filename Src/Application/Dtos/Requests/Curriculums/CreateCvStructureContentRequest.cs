using Backend.Src.Application.Dtos.Data.Curriculums;

namespace Backend.Src.Application.Dtos.Requests.Curriculums;

public record CreateCvStructuredContentRequest(
    string Title,
    bool IsCurrent,
    CvHeaderData Header,
    CvTextSectionData? Summary,
    List<CvExperienceItemData>? Experience,
    List<CvEducationItemData>? Education,
    CvTextSectionData? Skills,
    CvTextSectionData? Languages,
    CvTextSectionData? Certifications,
    CvTextSectionData? Projects,
    CvTextSectionData? Awards,
    List<CvCustomSectionData> CustomSections
);
using Backend.Src.Application.Dtos.Data.Curriculums;
using Backend.Src.Application.Dtos.Requests.Curriculums;
using Backend.Src.Application.Dtos.Responses.Curriculums;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Application.Mappers.Curriculums;

public static class CvStructuredContentMapper
{
    public static CvStructuredContent ToEntity(CreateCvStructuredContentRequest request, Guid cvId)
    {
        var header = new CvHeader(
            request.Header.FullName,
            request.Header.Headline,
            request.Header.Email,
            request.Header.Phone,
            request.Header.Location
        );

        var summary = request.Summary is null ? null : new CvTextSection(request.Summary.Title, request.Summary.Description);

        var experience = request.Experience is null
            ? null
            : new CvExperienceSection(
                request.Experience
                    .Select(x => new CvExperienceItem(
                        x.Employer,
                        x.Position,
                        x.StartDate,
                        x.EndDate,
                        x.Description
                    ))
                    .ToList()
            );

        var education = request.Education is null
            ? null
            : new CvEducationSection(
                request.Education
                    .Select(x => new CvEducationItem(
                        x.Institution,
                        x.Study,
                        x.AcademicLevel,
                        x.StartDate,
                        x.EndDate
                    ))
                    .ToList()
            );

        var skills = ToTextSection(request.Skills);
        var languages = ToTextSection(request.Languages);
        var certifications = ToTextSection(request.Certifications);
        var projects = ToTextSection(request.Projects);
        var awards = ToTextSection(request.Awards);

        var customSections = request.CustomSections
            .Select((section, index) =>
                new CvCustomSection(
                    section.Title,
                    section.Description,
                    index
                )
            )
            .ToList();

        return new CvStructuredContent(
            cvId,
            header,
            summary,
            experience,
            education,
            skills,
            languages,
            certifications,
            projects,
            awards,
            customSections
        );
    }

    public static CvStructuredContentResponse ToResponse(CvStructuredContent cv)
    {
        return new CvStructuredContentResponse(
            cv.Id,
            cv.UpdatedAt,
            new CvHeaderData(
                cv.Header.FullName,
                cv.Header.Headline,
                cv.Header.Email,
                cv.Header.Phone,
                cv.Header.Location
            ),
            cv.Summary is null ? null : new CvTextSectionData(cv.Summary.Title, cv.Summary.Description),
            cv.Experience is null
                ? []
                : cv.Experience.Items
                        .Select(item =>
                            new CvExperienceItemData(
                                item.Company,
                                item.Position,
                                item.StartDate,
                                item.EndDate,
                                item.Description
                            )
                        )
                        .ToList()
            ,
            cv.Education is null
                ? []
                : cv.Education.Items
                        .Select(item =>
                            new CvEducationItemData(
                                item.Institution,
                                item.FieldOfStudy,
                                item.Degree,
                                item.StartDate,
                                item.EndDate
                            )
                        )
                        .ToList(),
            ToTextSectionResponse(cv.Skills),
            ToTextSectionResponse(cv.Languages),
            ToTextSectionResponse(cv.Certifications),
            ToTextSectionResponse(cv.Projects),
            ToTextSectionResponse(cv.Awards),
            cv.CustomSections
                .OrderBy(x => x.Order)
                .Select(section =>
                    new CvCustomSectionData(
                        section.Title,
                        section.Description
                    )
                )
                .ToList()
        );
    }

    private static CvTextSection? ToTextSection(CvTextSectionData? section)
        => section is null ? null : new CvTextSection(section.Title, section.Description);

    private static CvTextSectionData? ToTextSectionResponse(CvTextSection? section)
        => section is null ? null : new CvTextSectionData(section.Title, section.Description);
}

using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.ValueObjects.Curriculums;
using Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Curriculums;

namespace Backend.Src.Infrastructure.Persistence.MongoDb.Mappers.Curriculums;

public static class CvDocumentMapper
{
    public static CvDocument ToDocument(CvStructuredContent cv)
    {
        return new CvDocument(
            cv.Id,
            cv.UpdatedAt,
            ToDocument(cv.Header),
            ToDocument(cv.Summary),
            ToDocument(cv.Experience),
            ToDocument(cv.Education),
            ToDocument(cv.Skills),
            ToDocument(cv.Languages),
            ToDocument(cv.Certifications),
            ToDocument(cv.Projects),
            ToDocument(cv.Awards),
            cv.CustomSections.Select(ToDocument).ToList()
        );
    }

    public static CvStructuredContent ToDomainEntity(CvDocument document)
    {
        return CvStructuredContent.Restore(
            document.Id,
            document.UpdatedAt,
            ToDomain(document.Header),
            ToDomain(document.Summary),
            ToDomain(document.Experience),
            ToDomain(document.Education),
            ToDomain(document.Skills),
            ToDomain(document.Languages),
            ToDomain(document.Certifications),
            ToDomain(document.Projects),
            ToDomain(document.Awards),
            document.CustomSections.Select(ToDomain).ToList()
        );
    }

    private static CvHeaderDocument ToDocument(CvHeader header)
    {
        return new CvHeaderDocument(
            header.FullName,
            header.Headline,
            header.Email,
            header.Phone,
            header.Location
        );
    }

    private static CvHeader ToDomain(CvHeaderDocument header)
    {
        return new CvHeader(
            header.FullName,
            header.Headline,
            header.Email,
            header.Phone,
            header.Location
        );
    }

    private static CvTextSectionDocument? ToDocument(CvTextSection? section)
    {
        if (section is null) return null;
        return new CvTextSectionDocument(section.Title, section.Description);
    }

    private static CvTextSection? ToDomain(CvTextSectionDocument? section)
    {
        if (section is null) return null;
        return new CvTextSection(section.Title, section.Description);
    }

    private static CvExperienceSectionDocument? ToDocument(CvExperienceSection? section)
    {
        if (section is null) return null;
        return new CvExperienceSectionDocument(
            section.Items
                .Select(item => new CvExperienceItemDocument(
                    item.Company,
                    item.Position,
                    item.StartDate,
                    item.EndDate,
                    item.Description
                )).ToList()
        );
    }

    private static CvExperienceSection? ToDomain(CvExperienceSectionDocument? section)
    {
        if (section is null) return null;

        return new CvExperienceSection(
            section.Items
                .Select(item => new CvExperienceItem(
                    item.Employer,
                    item.Position,
                    item.StartDate,
                    item.EndDate,
                    item.Description
                ))
                .ToList()
        );
    }

    private static CvEducationSectionDocument? ToDocument(CvEducationSection? section)
    {
        if (section is null) return null;

        return new CvEducationSectionDocument(
            section.Items
                .Select(item => new CvEducationItemDocument(
                    item.Institution,
                    item.FieldOfStudy,
                    item.Degree,
                    item.StartDate,
                    item.EndDate
                ))
                .ToList()
        );
    }

    private static CvEducationSection? ToDomain(CvEducationSectionDocument? section)
    {
        if (section is null) return null;

        return new CvEducationSection(
            section.Items
                .Select(item => new CvEducationItem(
                    item.Institution,
                    item.Study,
                    item.AcademicLevel,
                    item.StartDate,
                    item.EndDate
                ))
                .ToList()
        );
    }

    private static CvCustomSectionDocument ToDocument(CvCustomSection section)
    {
        return new CvCustomSectionDocument(
            section.Title,
            section.Description,
            section.Order
        );
    }

    private static CvCustomSection ToDomain(CvCustomSectionDocument section)
    {
        return new CvCustomSection(
            section.Title,
            section.Description,
            section.Order
        );
    }
}

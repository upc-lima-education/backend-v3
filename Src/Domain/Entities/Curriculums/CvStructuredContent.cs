using Backend.Src.Domain.Exceptions.Curriculums;
using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Domain.Entities.Curriculums;

public class CvStructuredContent
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public CvHeader Header { get; private set; } = new CvHeader();
    public CvTextSection? Summary { get; private set; }
    public CvExperienceSection? Experience { get; private set; }
    public CvEducationSection? Education { get; private set; }
    public CvTextSection? Skills { get; private set; }
    public CvTextSection? Languages { get; private set; }
    public CvTextSection? Certifications { get; private set; }
    public CvTextSection? Projects { get; private set; }
    public CvTextSection? Awards { get; private set; }
    public List<CvCustomSection> CustomSections { get; private set; } = [];

    public CvStructuredContent() { }

    public CvStructuredContent(
        Guid id,
        CvHeader header,
        CvTextSection? summary,
        CvExperienceSection? experience,
        CvEducationSection? education,
        CvTextSection? skills,
        CvTextSection? languages,
        CvTextSection? certifications,
        CvTextSection? projects,
        CvTextSection? awards,
        IReadOnlyList<CvCustomSection> customSections
    )
    {
        Id = id;
        Header = header;
        Summary = summary;
        Experience = experience;
        Education = education;
        Skills = skills;
        Languages = languages;
        Certifications = certifications;
        Projects = projects;
        Awards = awards;
        SetCustomSections(customSections);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        CvHeader header,
        CvTextSection? summary,
        CvExperienceSection? experience,
        CvEducationSection? education,
        CvTextSection? skills,
        CvTextSection? languages,
        CvTextSection? certifications,
        CvTextSection? projects,
        CvTextSection? awards,
        IReadOnlyList<CvCustomSection> customSections
    )
    {
        Header = header;
        Summary = summary;
        Experience = experience;
        Education = education;
        Skills = skills;
        Languages = languages;
        Certifications = certifications;
        Projects = projects;
        Awards = awards;
        SetCustomSections(customSections);
    }

    internal static CvStructuredContent Restore(
        Guid id,
        DateTime updatedAt,
        CvHeader header,
        CvTextSection? summary,
        CvExperienceSection? experience,
        CvEducationSection? education,
        CvTextSection? skills,
        CvTextSection? languages,
        CvTextSection? certifications,
        CvTextSection? projects,
        CvTextSection? awards,
        IReadOnlyList<CvCustomSection> customSections
    )
    {
        var content = new CvStructuredContent(
            id,
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
        content.UpdatedAt = updatedAt;
        return content;
    }

    public void MarkUpdated() => UpdatedAt = DateTime.UtcNow;

    public void UpdateHeader(CvHeader header) => Header = header;
    public void UpdateSummary(CvTextSection? summary) => Summary = summary;
    public void UpdateExperience(CvExperienceSection? experience) => Experience = experience;
    public void UpdateEducation(CvEducationSection? education) => Education = education;
    public void UpdateSkills(CvTextSection? skills) => Skills = skills;
    public void UpdateLanguages(CvTextSection? languages) => Languages = languages;
    public void UpdateCertifications(CvTextSection? certifications) => Certifications = certifications;
    public void UpdateProjects(CvTextSection? projects) => Projects = projects;
    public void UpdateAwards(CvTextSection? awards) => Awards = awards;

    public void SetCustomSections(IReadOnlyList<CvCustomSection> sections)
    {
        ValidateCustomSections(sections);
        CustomSections = sections.ToList();
    }

    private static void ValidateCustomSections(IReadOnlyList<CvCustomSection> sections)
    {
        var orders = sections
            .Select(x => x.Order)
            .OrderBy(x => x)
            .ToList();

        if (orders.Count != orders.Distinct().Count())
            throw new CustomSectionsAreNotUniqueException();
    }
}

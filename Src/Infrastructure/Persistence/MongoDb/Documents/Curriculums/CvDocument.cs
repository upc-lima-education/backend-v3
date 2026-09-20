using Backend.Src.Domain.Exceptions.Curriculums;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Curriculums;

public class CvDocument
{
    [BsonId]
    public Guid Id { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public CvHeaderDocument Header { get; private set; } = new();
    public CvTextSectionDocument? Summary { get; private set; }
    public CvExperienceSectionDocument? Experience { get; private set; }
    public CvEducationSectionDocument? Education { get; private set; }
    public CvTextSectionDocument? Skills { get; private set; }
    public CvTextSectionDocument? Languages { get; private set; }
    public CvTextSectionDocument? Certifications { get; private set; }
    public CvTextSectionDocument? Projects { get; private set; }
    public CvTextSectionDocument? Awards { get; private set; }
    public List<CvCustomSectionDocument> CustomSections { get; private set; } = [];

    public CvDocument(){}

    public CvDocument(
        Guid id,
        DateTime updatedAt,
        CvHeaderDocument header,
        CvTextSectionDocument? summary,
        CvExperienceSectionDocument? experience,
        CvEducationSectionDocument? education,
        CvTextSectionDocument? skills,
        CvTextSectionDocument? languages,
        CvTextSectionDocument? certifications,
        CvTextSectionDocument? projects,
        CvTextSectionDocument? awards,
        IReadOnlyList<CvCustomSectionDocument> customSections
    )
    {
        Id = id;
        UpdatedAt = updatedAt;
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

    public void SetCustomSections(IReadOnlyList<CvCustomSectionDocument> sections)
    {
        ValidateCustomSections(sections);
        CustomSections = sections.ToList();
    }

    private static void ValidateCustomSections(IReadOnlyList<CvCustomSectionDocument> sections)
    {
        var orders = sections
            .Select(x => x.Order)
            .OrderBy(x => x)
            .ToList();

        if (orders.Count != orders.Distinct().Count())
            throw new CustomSectionsAreNotUniqueException();
    }
}

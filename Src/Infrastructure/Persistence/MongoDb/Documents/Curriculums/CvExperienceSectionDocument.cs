namespace Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Curriculums;

public class CvExperienceSectionDocument(List<CvExperienceItemDocument> items)
{
    public List<CvExperienceItemDocument> Items { get; private set; } = items;
}

public class CvExperienceItemDocument(string employer, string position, DateOnly? startDate, DateOnly? endDate, string description)
{
    public string Employer { get; private set; } = employer;
    public string Position { get; private set; } = position;
    public DateOnly? StartDate { get; private set; } = startDate;
    public DateOnly? EndDate { get; private set; } = endDate;
    public string Description { get; private set; } = description;
}
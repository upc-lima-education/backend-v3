namespace Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Curriculums;

public class CvEducationSectionDocument(List<CvEducationItemDocument> items)
{
    public List<CvEducationItemDocument> Items { get; private set; } = items;
}

public class CvEducationItemDocument(string institution, string study, string academicLevel, DateOnly? startDate, DateOnly? endDate)
{
    public string Institution { get; private set; } = institution;
    public string Study { get; private set; } = study;
    public string AcademicLevel { get; private set; } = academicLevel;
    public DateOnly? StartDate { get; private set; } = startDate;
    public DateOnly? EndDate { get; private set; } = endDate;
}
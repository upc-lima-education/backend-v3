namespace Backend.Src.Domain.ValueObjects.Curriculums;

public class CvEducationSection(List<CvEducationItem> items)
{
    public List<CvEducationItem> Items { get; private set; } = items;
}

public class CvEducationItem(string institution, string study, string academicLevel, DateOnly? startDate, DateOnly? endDate)
{
    public string Institution { get; private set; } = institution;
    public string FieldOfStudy { get; private set; } = study;
    public string Degree { get; private set; } = academicLevel;
    public DateOnly? StartDate { get; private set; } = startDate;
    public DateOnly? EndDate { get; private set; } = endDate;
}
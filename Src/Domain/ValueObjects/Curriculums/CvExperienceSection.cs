namespace Backend.Src.Domain.ValueObjects.Curriculums;

public class CvExperienceSection(List<CvExperienceItem> items)
{
    public List<CvExperienceItem> Items { get; private set; } = items;
}

public class CvExperienceItem(string company, string position, DateOnly? startDate, DateOnly? endDate, string description)
{
    public string Company { get; private set; } = company;
    public string Position { get; private set; } = position;
    public DateOnly? StartDate { get; private set; } = startDate;
    public DateOnly? EndDate { get; private set; } = endDate;
    public string Description { get; private set; } = description;
}
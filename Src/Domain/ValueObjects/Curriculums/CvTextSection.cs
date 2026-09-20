namespace Backend.Src.Domain.ValueObjects.Curriculums;

public class CvTextSection(string title, string description)
{
    public string Title { get; private set; } = title;
    public string Description { get; private set; } = description;
}
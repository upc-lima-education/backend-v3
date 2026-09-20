namespace Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Curriculums;

public class CvCustomSectionDocument(string title, string description, int order)
{
    public string Title { get; private set; } = title;
    public string Description { get; private set; } = description;
    public int Order { get; private set; } = order;
}
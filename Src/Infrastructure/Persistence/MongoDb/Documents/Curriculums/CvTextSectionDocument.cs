namespace Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Curriculums;

public class CvTextSectionDocument(string title, string description)
{
    public string Title { get; private set; } = title;
    public string Description { get; private set; } = description;
}
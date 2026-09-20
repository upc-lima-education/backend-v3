namespace Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Curriculums;

public class CvHeaderDocument
{
    public string FullName { get; private set; } = string.Empty;
    public string Headline { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Location { get; private set; }

    public CvHeaderDocument(){}

    public CvHeaderDocument(
        string fullName,
        string headline,
        string? email,
        string? phone,
        string? location
    )
    {
        FullName = fullName;
        Headline = headline;
        Email = email;
        Phone = phone;
        Location = location;
    }
}
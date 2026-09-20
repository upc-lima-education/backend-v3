namespace Backend.Src.Domain.ValueObjects.Curriculums;

public class CvHeader
{
    public string FullName { get; private set; } = string.Empty;
    public string Headline { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Location { get; private set; }

    public CvHeader() {}

    public CvHeader(
        string fullname,
        string headline,
        string? email,
        string? phone,
        string? location
    )
    {
        FullName = fullname;
        Headline = headline;
        Email = email;
        Phone = phone;
        Location = location;
    }
}
namespace Backend.Src.Domain.Entities.Profiles;

public sealed class Education
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CandidateProfileId { get; private set; }
    public string Institution { get; private set; } = string.Empty;
    public string Degree { get; private set; } = string.Empty;
    public string? FieldOfStudy { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    
    public Education() {}

    public Education(
        Guid candidateProfileId,
        string institution,
        string degree,
        string? fieldOfStudy,
        DateOnly startDate,
        DateOnly? endDate
    )
    {
        Id = Guid.NewGuid();
        CandidateProfileId = candidateProfileId;
        Institution = institution;
        Degree = degree;
        FieldOfStudy = fieldOfStudy;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void Update(
        string institution,
        string degree,
        string? fieldOfStudy,
        DateOnly startDate,
        DateOnly? endDate
    )
    {
        Institution = institution;
        Degree = degree;
        FieldOfStudy = fieldOfStudy;
        StartDate = startDate;
        EndDate = endDate;
    }
}
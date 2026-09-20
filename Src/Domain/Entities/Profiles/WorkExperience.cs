namespace Backend.Src.Domain.Entities.Profiles;

public sealed class WorkExperience
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CandidateProfileId { get; private set; } = Guid.NewGuid();
    public string Company { get; private set; } = string.Empty;
    public string Position { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public WorkExperience() {}

    public WorkExperience(
        Guid candidateProfileId,
        string company,
        string position,
        string? description,
        DateOnly startDate,
        DateOnly? endDate
    )
    {
        Id = Guid.NewGuid();
        CandidateProfileId = candidateProfileId;
        Company = company;
        Position = position;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void Update(
        string company,
        string position,
        string description,
        DateOnly startDate,
        DateOnly? endDate
    )
    {
        Company = company;
        Position = position;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }
}
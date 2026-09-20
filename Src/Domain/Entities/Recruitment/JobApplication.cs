using Backend.Src.Domain.Exceptions.Recruitment;
using Backend.Src.Domain.ValueObjects.Recruitment;

namespace Backend.Src.Domain.Entities.Recruitment;

public class JobApplication
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid JobId { get; private set; }
    public Guid CandidateId { get; private set; }
    public string CvStorageKey { get; private set; } = string.Empty;
    public ApplicationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public JobApplication() {}

    public JobApplication(
        Guid jobId,
        Guid candidateId,
        string cvUrl
    )
    {
        Id = Guid.NewGuid();
        JobId = jobId;
        CandidateId = candidateId;
        CvStorageKey = cvUrl;
        Status = ApplicationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve() => Transition(ApplicationStatus.Accepted);
    public void Reject() => Transition(ApplicationStatus.Rejected);

    private void Transition(ApplicationStatus status)
    {
        if (Status != ApplicationStatus.Pending)
            throw new InvalidApplicationStatusTransitionException(Status, status);
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
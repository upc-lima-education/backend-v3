using Backend.Src.Domain.ValueObjects.Recommendation;

namespace Backend.Src.Domain.Entities.Recommendation;

public sealed class JobInteraction
{
    public Guid Id { get; private set; }
    public Guid CandidateProfileId { get; private set; }
    public Guid JobId { get; private set; }
    public JobInteractionType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private JobInteraction() { }

    public JobInteraction(Guid candidateProfileId, Guid jobId, JobInteractionType type)
    {
        if (candidateProfileId == Guid.Empty) throw new ArgumentException("Candidate profile is required.", nameof(candidateProfileId));
        if (jobId == Guid.Empty) throw new ArgumentException("Job is required.", nameof(jobId));

        Id = Guid.NewGuid();
        CandidateProfileId = candidateProfileId;
        JobId = jobId;
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }
}

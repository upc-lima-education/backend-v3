using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Domain.Entities.Curriculums;

public class Cv
{
    public Guid Id { get; private set; }
    public Guid CandidateId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public bool IsCurrent { get; private set; }
    //Traceability
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    //Content
    public string? FileContentKey { get; private set; }
    public CvProcessingStatus ProcessingStatus { get; private set; } = CvProcessingStatus.Ready;
    public string? ProcessingError { get; private set; }

    public Cv() { }

    public Cv(
        Guid candidateId,
        string title,
        bool isCurrent
    )
    {
        Id = Guid.NewGuid();
        CandidateId = candidateId;
        Title = title;
        IsCurrent = isCurrent;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public Cv(
        Guid id,
        Guid candidateId,
        string title,
        bool isCurrent
    )
    {
        Id = id;
        CandidateId = candidateId;
        Title = title;
        IsCurrent = isCurrent;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void PatchTitle(string title)
    {
        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsCurrent()
    {
        IsCurrent = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnsetAsCurrent()
    {
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFileContent(string key)
    {
        FileContentKey = key;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkProcessing()
    {
        ProcessingStatus = CvProcessingStatus.Processing;
        ProcessingError = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkReady()
    {
        ProcessingStatus = CvProcessingStatus.Ready;
        ProcessingError = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string? error = null)
    {
        ProcessingStatus = CvProcessingStatus.Failed;
        ProcessingError = string.IsNullOrWhiteSpace(error)
            ? "No se pudo procesar el CV."
            : error[..Math.Min(error.Length, 500)];
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void RemoveUploadedContent()
    {
        FileContentKey = null;
        UpdatedAt = DateTime.UtcNow;
    }
}

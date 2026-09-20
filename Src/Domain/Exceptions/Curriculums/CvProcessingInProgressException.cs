using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Curriculums;

public sealed class CvProcessingInProgressException(Guid cvId)
    : DomainException($"CV '{cvId}' is still being processed.")
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "CV processing in progress";
}

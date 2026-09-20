using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Curriculums;

public sealed class CvAccessDeniedException(Guid cvId)
    : DomainException($"You do not have access to CV '{cvId}'.")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "CV access denied";
}
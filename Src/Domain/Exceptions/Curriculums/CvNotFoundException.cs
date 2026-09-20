using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Curriculums;

public sealed class CvNotFoundException(Guid cvId)
    : DomainException($"CV '{cvId}' was not found.")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "CV not found";
}
using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Curriculums;

public sealed class CvContentTypeMismatchException(Guid cvId)
    : DomainException($"CV '{cvId}' does not contain the solicited content")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "CV ContentType Mismatch";
}
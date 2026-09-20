using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Curriculums;

public sealed class CvStructuredContentNotFoundException(Guid cvStructuredContentId)
    : DomainException($"CV Content with id '{cvStructuredContentId}' was not found.")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "CV Structured Content not found";
}
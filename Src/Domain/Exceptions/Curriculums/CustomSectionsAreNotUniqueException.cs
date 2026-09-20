using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Curriculums;

public sealed class CustomSectionsAreNotUniqueException()
    : DomainException("Custom section orders must be unique.")
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Sections are not unique";
}
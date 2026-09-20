using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Curriculums;

public sealed class CvUploadedContentNotFoundException(Guid cvUploadedContentKey)
    : DomainException($"CV Blob with key '{cvUploadedContentKey}' was not found.")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "CV Uploaded Content not found";
}
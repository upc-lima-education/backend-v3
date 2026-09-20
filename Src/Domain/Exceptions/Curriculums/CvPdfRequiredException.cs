using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Curriculums;

public sealed class CvPdfRequiredException(Guid cvId)
    : DomainException($"CV '{cvId}' is not a PDF. Upload a PDF version to download it from Llanqui.")
{
    public override int StatusCode => StatusCodes.Status415UnsupportedMediaType;
    public override string Title => "PDF required";
}

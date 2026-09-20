using Backend.Src.Application.Dtos.Requests.Curriculums;
using Backend.Src.Domain.Rules.Curriculums;
using FluentValidation;

namespace Backend.Src.Application.Validators.Curriculums;

internal sealed class CreateCvUploadedContentValidator : AbstractValidator<CreateCvUploadedContentRequest>
{
    public CreateCvUploadedContentValidator()
    {
        RuleFor(x => x.Cv.File.Content.Length)
            .LessThanOrEqualTo(CvUploadedContentRules.MaxFileSizeBytes);
        
        RuleFor(x => x.Cv.File.ContentType)
            .Equal(CvUploadedContentRules.PdfContentType)
            .WithMessage("Only PDF files are allowed.");

        RuleFor(x => x.Cv.File.FileName)
            .Must(fileName => Path.GetExtension(fileName).Equals(CvUploadedContentRules.PdfExtension, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only PDF files are allowed.");
    }
}

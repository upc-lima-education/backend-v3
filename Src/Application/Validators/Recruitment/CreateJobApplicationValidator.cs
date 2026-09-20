using Backend.Src.Application.Dtos.Requests.Recruitment;
using Backend.Src.Domain.Rules.Curriculums;
using FluentValidation;

namespace Backend.Src.Application.Validators.Recruitment;

internal sealed class CreateJobApplicationValidator : AbstractValidator<CreateJobApplicationRequest>
{
    public CreateJobApplicationValidator()
    {
        RuleFor(request => request.CvContent)
            .NotNull()
            .Must(stream => stream.CanRead && stream.Length > 0)
            .WithMessage("A non-empty CV file is required.")
            .Must(stream => stream.Length <= CvUploadedContentRules.MaxFileSizeBytes)
            .WithMessage("The CV PDF must be 2 MB or smaller.");

        RuleFor(request => request.ContentType)
            .Equal(CvUploadedContentRules.PdfContentType)
            .WithMessage("Only PDF files are allowed.");

        RuleFor(request => request.FileName)
            .Must(fileName => Path.GetExtension(fileName).Equals(CvUploadedContentRules.PdfExtension, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only PDF files are allowed.");
    }
}

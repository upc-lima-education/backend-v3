using Backend.Src.Domain.Rules.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using FluentValidation;

namespace Backend.Src.Application.Validators.Jobs;

internal sealed class UpdateJobValidator : AbstractValidator<UpdateJobRequest>
{

    public UpdateJobValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(JobRules.MinTitleLength)
            .MaximumLength(JobRules.MaxTitleLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(JobRules.MinDescriptionLength)
            .MaximumLength(JobRules.MaxDescriptionLength);

        RuleFor(x => x.Skills)
            .Must(s => s.Count <= JobRules.MaxSkillsAmount).WithMessage($"A job must have at most {JobRules.MaxSkillsAmount} skills");

        RuleFor(x => x.OpensAt)
            .NotEmpty().WithMessage("Opening date is required");

        RuleFor(x => x.ClosesAt)
            .GreaterThan(x => x.OpensAt).WithMessage("Closing date must be after opening date");

        RuleFor(x => x.ApplyUrl)
            .Must(BeAValidExternalUrl)
            .WithMessage("Apply URL must be an absolute HTTP or HTTPS URL.");

        RuleFor(x => x.Location)
            .SetValidator(new JobLocationValidator()!);

        RuleFor(x => x.Payment)
            .SetValidator(new JobPaymentValidator()!);

    }

    private static bool BeAValidExternalUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            || Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}

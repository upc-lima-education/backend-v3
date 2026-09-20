using Backend.Src.Domain.Rules.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using FluentValidation;
namespace Backend.Src.Application.Validators.Jobs;

public sealed class CreateInternalJobValidator : AbstractValidator<CreateInternalJobRequest>
{
    public CreateInternalJobValidator()
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
            .Must(s => s.Count <= JobRules.MaxSkillsAmount)
            .WithMessage($"A job must have at most {JobRules.MaxSkillsAmount} skills");

        RuleFor(x => x.OpensAt)
            // Una empresa puede publicar una vacante para que esté activa de
            // inmediato. La tolerancia absorbe el desfase entre el cliente y
            // el servidor sin permitir programaciones históricas reales.
            .GreaterThanOrEqualTo(_ => DateTime.UtcNow.AddDays(-1))
            .WithMessage("Opening date cannot be more than one day in the past");

        RuleFor(x => x.ClosesAt)
            .GreaterThan(x => x.OpensAt)
            .WithMessage("Closing date must be after opening date");

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

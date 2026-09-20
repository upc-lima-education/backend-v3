using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Rules.Profiles;
using FluentValidation;

namespace Backend.Src.Application.Validators.Profiles;

internal sealed class UpdateCompanyProfileValidator : AbstractValidator<UpdateCompanyProfileRequest>
{
    private const int MaxCompanyLength = CompanyProfileRules.MaxCompanyNameLength;
    public UpdateCompanyProfileValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name cannot be empty")
            .MaximumLength(MaxCompanyLength).WithMessage($"Company name must be at most {MaxCompanyLength} characters long");
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+[1-9]\d{6,14}$")
            .When(x => x.PhoneNumber is not null)
            .WithMessage("Must be a valid phone number");
        RuleFor(x => x.Sector)
            .MaximumLength(CompanyProfileRules.MaxSectorLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Sector));
        RuleFor(x => x.Website)
            .MaximumLength(CompanyProfileRules.MaxWebsiteLength)
            .Must(BeAValidWebsite)
            .When(x => !string.IsNullOrWhiteSpace(x.Website))
            .WithMessage("Website must be an absolute HTTP or HTTPS URL.");
        RuleFor(x => x.CompanySize)
            .MaximumLength(CompanyProfileRules.MaxCompanySizeLength)
            .Must(BeAValidCompanySize)
            .When(x => !string.IsNullOrWhiteSpace(x.CompanySize))
            .WithMessage("Company size is not supported.");
    }

    private static bool BeAValidWebsite(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    private static bool BeAValidCompanySize(string? value) => value is "1-10" or "11-50" or "51-200" or "201-500" or "501+";
}

using Backend.Src.Application.Dtos.Data.Jobs;
using Backend.Src.Domain.Rules.Jobs;
using FluentValidation;

namespace Backend.Src.Application.Validators.Jobs;

internal sealed class JobLocationValidator : AbstractValidator<JobLocationData>
{
    public JobLocationValidator()
    {
        RuleFor(x => x.Ubigeo)
            .Length(JobRules.UbigeoLength)
            .When(x => x.Ubigeo is not null);

        RuleFor(x => x.Address)
            .MaximumLength(JobRules.MaxAddressLength)
            .When(x => x.Address is not null);
    }
}
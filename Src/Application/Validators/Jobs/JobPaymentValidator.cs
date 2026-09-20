using Backend.Src.Application.Dtos.Data.Jobs;
using Backend.Src.Domain.Rules.Jobs;
using FluentValidation;

namespace Backend.Src.Application.Validators.Jobs;

internal sealed class JobPaymentValidator : AbstractValidator<JobPaymentData>
{
    public JobPaymentValidator()
    {
        RuleFor(x => x.MaxSalary)
            .GreaterThanOrEqualTo(x => x.MinSalary)
            .When(x => x.MaxSalary.HasValue && x.MinSalary.HasValue);
    }
}
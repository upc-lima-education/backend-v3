using Backend.Src.Application.Dtos.Requests.Jobs;
using FluentValidation;

namespace Backend.Src.Application.Validators.Jobs;

internal sealed class PatchJobScheduleValidator : AbstractValidator<PatchJobScheduleRequest>
{
    public PatchJobScheduleValidator()
    {
        RuleFor(x => x.ClosesAt)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Closing date cannot be in the past");
    }
}
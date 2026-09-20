using Backend.Src.Domain.Rules.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using FluentValidation;

namespace Backend.Src.Application.Validators.Jobs;

internal sealed class PatchJobSkillsValidator : AbstractValidator<PatchJobSkillsRequest>
{
    private const int MaxSkillsLength = JobRules.MaxSkillsAmount;

    public PatchJobSkillsValidator()
    {
        RuleFor(x => x.Skills)
            .Must(s => s.Count < MaxSkillsLength).WithMessage($"Skill lenght must be at most {MaxSkillsLength}");
    }
}
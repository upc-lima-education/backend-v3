using Backend.Src.Application.Dtos.Requests.Recommendation;
using FluentValidation;

namespace Backend.Src.Application.Validators.Recommendation;

public sealed class SearchRecommendationsValidator : AbstractValidator<SearchRecommendationsRequest>
{
    public SearchRecommendationsValidator()
    {
        RuleFor(request => request.Query).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Page).GreaterThanOrEqualTo(1);
        RuleFor(request => request.PageSize).InclusiveBetween(1, 50);
        RuleFor(request => request.MinSalary)
            .GreaterThanOrEqualTo(0)
            .When(request => request.MinSalary.HasValue);
    }
}

using Backend.Src.Application.Dtos.Requests.Recommendation;
using Backend.Src.Application.Dtos.Responses.Common;
using Backend.Src.Application.Dtos.Responses.Recommendation;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Infrastructure.Adapters.Recommendation;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Recommendation;

public sealed class SearchRecommendationsUseCase(
    IJobRepository jobRepository,
    IRecommendationClient recommendationClient,
    IValidator<SearchRecommendationsRequest> validator)
{
    public async Task<PagedResponse<RecommendationJobResponse>> ExecuteAsync(
        SearchRecommendationsRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var recommendationPage = await recommendationClient.GetCbfRecommendationsAsync(request, cancellationToken);
        var orderedIds = recommendationPage.Items
            .Where(item => item.Score > 0 && Guid.TryParse(item.JobId, out _))
            .Select(item => Guid.Parse(item.JobId))
            .Distinct()
            .ToList();

        var jobsById = (await jobRepository.GetByIdsAsync(orderedIds)).ToDictionary(job => job.Id);
        var items = recommendationPage.Items
            .Where(item => item.Score > 0 && Guid.TryParse(item.JobId, out _))
            .Select(item =>
            {
                var jobId = Guid.Parse(item.JobId);
                if (!jobsById.TryGetValue(jobId, out var job)) return null;
                return new RecommendationJobResponse(
                    job.Id,
                    job.Title,
                    job.Company?.CompanyName ?? job.ExternalCompanyName ?? string.Empty,
                    job.Ubigeo,
                    job.MinSalary,
                    job.MaxSalary,
                    job.SourceUrl,
                    item.Score,
                    job.JobType.ToString(),
                    job.Company?.Profile?.ProfilePicture ?? job.ExternalCompanyImage);
            })
            .Where(item => item is not null)
            .Cast<RecommendationJobResponse>()
            .ToList();

        return new PagedResponse<RecommendationJobResponse>(
            items, recommendationPage.TotalItems, recommendationPage.Page, recommendationPage.PageSize);
    }
}

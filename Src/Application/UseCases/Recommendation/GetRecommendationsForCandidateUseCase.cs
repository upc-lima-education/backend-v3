using Backend.Src.Application.Dtos.Responses.Recommendation;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Infrastructure.Adapters.Recommendation;
using Microsoft.Extensions.Logging;

namespace Backend.Src.Application.UseCases.Recommendation;

public sealed class GetRecommendationsForCandidateUseCase(
    IJobRepository jobRepository,
    IProfileRepository profileRepository,
    IRecommendationClient recommendationClient,
    ILogger<GetRecommendationsForCandidateUseCase> logger)
{
    public async Task<IReadOnlyList<RecommendationJobResponse>> ExecuteAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        limit = Math.Clamp(limit, 1, 50);
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        IReadOnlyList<AlsRecommendationItem> results;
        try
        {
            results = await recommendationClient.GetAlsRecommendationsAsync(
                profile.CandidateProfile.ProfileId.ToString(), limit, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[Recommendation] Error connecting to recommendation service. Falling back to recent active jobs.");
            return await GetRecentJobsFallbackAsync(limit, cancellationToken);
        }

        var validItems = results
            .Where(r => r.Score > 0 && Guid.TryParse(r.JobId, out _))
            .ToList();

        if (validItems.Count == 0)
        {
            return await GetRecentJobsFallbackAsync(limit, cancellationToken);
        }

        var ids = validItems.Select(r => Guid.Parse(r.JobId)).Distinct().ToList();
        var jobs = await jobRepository.GetByIdsAsync(ids);
        var jobsById = jobs.ToDictionary(j => j.Id);

        var hydrated = validItems
            .Select(item =>
            {
                var id = Guid.Parse(item.JobId);
                return jobsById.TryGetValue(id, out var job)
                    ? new RecommendationJobResponse(
                        id,
                        job.Title,
                        job.Company?.CompanyName ?? job.ExternalCompanyName ?? string.Empty,
                        job.Ubigeo,
                        job.MinSalary,
                        job.MaxSalary,
                        job.SourceUrl,
                        item.Score,
                        job.JobType.ToString(),
                        job.Company?.Profile?.ProfilePicture ?? job.ExternalCompanyImage)
                    : null;
            })
            .Where(r => r is not null)
            .Cast<RecommendationJobResponse>()
            .ToList();

        return hydrated.Count > 0 ? hydrated : await GetRecentJobsFallbackAsync(limit, cancellationToken);
    }

    private async Task<IReadOnlyList<RecommendationJobResponse>> GetRecentJobsFallbackAsync(int limit, CancellationToken cancellationToken)
    {
        var recentJobs = await jobRepository.GetRecentActiveJobsAsync(limit, cancellationToken);
        return recentJobs.Select(job => new RecommendationJobResponse(
            job.Id,
            job.Title,
            job.Company?.CompanyName ?? job.ExternalCompanyName ?? string.Empty,
            job.Ubigeo,
            job.MinSalary,
            job.MaxSalary,
            job.SourceUrl,
            0.5,
            job.JobType.ToString(),
            job.Company?.Profile?.ProfilePicture ?? job.ExternalCompanyImage
        )).ToList();
    }
}

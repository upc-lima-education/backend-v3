using Backend.Src.Domain.Entities.Jobs;
using Backend.Src.Domain.Entities.Skills;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.ValueObjects.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using Backend.Src.Application.Dtos.Responses.Jobs;
using Backend.Src.Application.Resolvers.Skills;

namespace Backend.Src.Application.UseCases.Jobs;

public class SyncScrapedJobsUseCase(
    IJobRepository jobRepository,
    SkillResolver skillResolver
)
{
    public async Task<SyncScrapedJobsResponse> ExecuteAsync(IEnumerable<ScrapedJobRequest> requests)
    {
        var requestList = requests.ToList();
        if (requestList.Count == 0) return new SyncScrapedJobsResponse(0, 0);

        var incomingUrls = requestList
            .Where(r => !string.IsNullOrWhiteSpace(r.SourceUrl))
            .Select(r => r.SourceUrl);

        var existingUrls = await jobRepository.GetExistingSourceUrlsAsync(incomingUrls);

        // Pre-resolver todas las habilidades unicas del lote completo en una sola operacion
        var allSkillNames = requestList
            .SelectMany(r => r.Skills ?? Enumerable.Empty<string>())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .ToList();

        var resolvedSkills = await skillResolver.ResolveAsync(allSkillNames);
        var skillMap = resolvedSkills
            .GroupBy(s => s.Name.ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.First());

        var newJobs = new List<Job>();
        int jobsSkipped = 0;
        var now = DateTime.UtcNow;

        foreach (var request in requestList)
        {
            if (string.IsNullOrWhiteSpace(request.SourceUrl) || existingUrls.Contains(request.SourceUrl))
            {
                jobsSkipped++;
                continue;
            }

            // Registrar en el conjunto para evitar duplicados dentro del mismo lote
            existingUrls.Add(request.SourceUrl);

            var skills = (request.Skills ?? [])
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLowerInvariant())
                .Distinct()
                .Where(skillMap.ContainsKey)
                .Select(s => skillMap[s])
                .ToList();

            var closesAt = request.ClosesAt ?? now.AddDays(30);

            var job = new Job(
                null,
                //Details
                request.Title,
                request.Description,
                request.JobType,
                request.WorkHours,
                //Requirements
                skills,
                request.Experience,
                request.EducationLevel,
                //Location
                request.Ubigeo,
                request.Address,
                //Payment
                request.MinSalary,
                request.MaxSalary,
                request.Currency,
                request.SalaryPeriod,
                request.CompensationType,
                //Traceability
                request.OpensAt ?? now,
                closesAt,
                request.OriginPage,
                //External
                request.ExternalCompanyName ?? "Confidencial",
                sourceUrl: request.SourceUrl,
                applyUrl: null,
                externalCompanyImage: request.ExternalCompanyImage
            );
            newJobs.Add(job);
        }

        if (newJobs.Count > 0) await jobRepository.CreateBatchAsync(newJobs);
        var response = new SyncScrapedJobsResponse(newJobs.Count, jobsSkipped);
        return response;
    }
}

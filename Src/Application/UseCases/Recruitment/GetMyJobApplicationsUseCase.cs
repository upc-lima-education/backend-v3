using Backend.Src.Application.Dtos.Responses.Recruitment;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Repositories.Recruitment;

namespace Backend.Src.Application.UseCases.Recruitment;

public class GetMyJobApplicationsUseCase(
    IJobApplicationRepository applicationRepository,
    IJobRepository jobRepository,
    IProfileRepository profileRepository
)
{
    public async Task<IReadOnlyList<CandidateJobApplicationResponse>> ExecuteAsync(Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);

        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var applications = await applicationRepository.GetByCandidateIdAsync(profile.Id);

        var responses = new List<CandidateJobApplicationResponse>();
        foreach (var app in applications)
        {
            var job = await jobRepository.GetByIdAsync(app.JobId);
            var jobTitle = job?.Title ?? "Vacante no disponible";
            var companyName = job?.ExternalCompanyName ?? job?.Company?.CompanyName;

            responses.Add(new CandidateJobApplicationResponse(
                app.Id,
                app.JobId,
                jobTitle,
                companyName,
                app.Status.ToString(),
                app.CreatedAt,
                app.UpdatedAt
            ));
        }

        return responses;
    }
}

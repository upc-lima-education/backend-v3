using Backend.Src.Application.Dtos.Responses.Recruitment;
using Backend.Src.Application.Mappers.Recruitment;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Exceptions.Recruitment;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Repositories.Recruitment;

namespace Backend.Src.Application.UseCases.Recruitment;

public class GetJobApplicationsByJobUseCase(
    IJobApplicationRepository applicationRepository,
    IJobRepository jobRepository,
    IProfileRepository profileRepository
)
{
    public async Task<IReadOnlyList<JobApplicationResponse>> ExecuteAsync(Guid jobId, Guid userId)
    {
        var job = await jobRepository.GetByIdAsync(jobId)
            ?? throw new JobNotFoundException(jobId);

        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if(profile.CompanyProfile is null) throw new CompanyProfileRequiredException();

        if (job.CompanyId != profile.Id)
            throw new JobApplicationAccessDeniedException();

        var applications = await applicationRepository.GetByJobIdAsync(jobId);
        
        var responses = new List<JobApplicationResponse>();
        foreach (var app in applications)
        {
            var candidateProfile = await profileRepository.GetByIdAsync(app.CandidateId);
            CandidateApplicationSummaryData? candidateData = null;
            if (candidateProfile?.CandidateProfile is not null)
            {
                candidateData = new CandidateApplicationSummaryData(
                    candidateProfile.CandidateProfile.FirstName,
                    candidateProfile.CandidateProfile.LastName,
                    candidateProfile.ProfilePicture,
                    candidateProfile.PhoneNumber,
                    candidateProfile.Skills.Select(s => s.Name).ToList()
                );
            }
            responses.Add(JobApplicationMapper.ToResponse(app, candidateData));
        }

        return responses;
    }
}
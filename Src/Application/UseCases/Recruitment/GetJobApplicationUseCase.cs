using Backend.Src.Application.Dtos.Responses.Common;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Exceptions.Recruitment;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Repositories.Recruitment;

namespace Backend.Src.Application.UseCases.Recruitment;

public class GetJobApplicationUseCase(
    IJobApplicationRepository applicationRepository,
    IProfileRepository profileRepository,
    IJobRepository jobRepository,
    IFileStoragePort fileStoragePort
)
{
    public async Task<BlobResponse> ExecuteAsync(Guid applicationId, Guid userId)
    {
        var jobApplication = await applicationRepository.GetByIdAsync(applicationId)
            ?? throw new JobApplicationNotFoundException(applicationId);

        var job = await jobRepository.GetByIdAsync(jobApplication.JobId)
            ?? throw new JobNotFoundException(jobApplication.JobId);
        
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        
        if (profile.CompanyProfile is null)
            throw new CompanyProfileRequiredException();
        if (job.CompanyId != profile.Id)
            throw new JobApplicationAccessDeniedException();

        var stream = await fileStoragePort.DownloadAsync(jobApplication.CvStorageKey);

        return new BlobResponse(
            stream,
            "application/pdf",
            $"{jobApplication}.pdf"
        );
    }
}
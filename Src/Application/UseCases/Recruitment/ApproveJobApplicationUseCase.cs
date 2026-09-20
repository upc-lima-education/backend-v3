using Backend.Src.Application.Dtos.Requests.Recruitment;
using Backend.Src.Application.Dtos.Responses.Recruitment;
using Backend.Src.Application.Mappers.Recruitment;
using Backend.Src.Domain.Contracts.MessageBroker.Recruitment;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Exceptions.Recruitment;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Repositories.Recruitment;

namespace Backend.Src.Application.UseCases.Recruitment;

public class ApproveJobApplicationUseCase(
    IJobApplicationRepository applicationRepository,
    IJobRepository jobRepository,
    IProfileRepository profileRepository,
    IMessageProducerPort messageProducer
)
{
    public async Task<JobApplicationResponse> ExecuteAsync(Guid applicationId, ApproveJobApplicationRequest request, Guid userId)
    {
        var application = await applicationRepository.GetByIdForUpdateAsync(applicationId)
            ?? throw new JobApplicationNotFoundException(applicationId);

        var job = await jobRepository.GetByIdAsync(application.JobId)
            ?? throw new JobNotFoundException(application.JobId);

        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CompanyProfile is null)
            throw new CompanyProfileRequiredException();
        if (job.CompanyId != profile.Id)
            throw new JobAccessDeniedException(job.Id);

        application.Approve();

        await applicationRepository.UpdateAsync(application);

        var candidate = await profileRepository.GetByIdAsync(application.CandidateId)
            ?? throw new ProfileNotFoundException(application.CandidateId);

        var message = new ApproveJobApplicationMessage(
            candidate.Id,
            job.Title,
            profile.CompanyProfile.CompanyName,
            request.Channels
        );

        await messageProducer.PublishAsync(message);

        return JobApplicationMapper.ToResponse(application);
    }
}
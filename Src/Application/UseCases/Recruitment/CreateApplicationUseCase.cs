using Backend.Src.Application.Dtos.Requests.Recruitment;
using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Entities.Recommendation;
using Backend.Src.Domain.Entities.Recruitment;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Exceptions.Recruitment;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Repositories.Recommendation;
using Backend.Src.Domain.Repositories.Recruitment;
using Backend.Src.Domain.ValueObjects.Jobs;
using Backend.Src.Domain.ValueObjects.Recommendation;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Recruitment;

public class CreateJobApplicationUseCase(
    IJobApplicationRepository applicationRepository,
    IJobRepository jobRepository,
    IProfileRepository profileRepository,
    IFileStoragePort fileStoragePort,
    IJobInteractionRepository interactionRepository,
    IValidator<CreateJobApplicationRequest> validator
)
{
    public async Task<Guid> ExecuteAsync(CreateJobApplicationRequest request, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var job = await jobRepository.GetByIdAsync(request.JobId)
            ?? throw new JobNotFoundException(request.JobId);
        if (job.JobStatus != JobStatus.Active)
            throw new JobNotActiveException(request.JobId);
        if (job.OriginPage != OriginPage.Internal || job.CompanyId is null || !string.IsNullOrWhiteSpace(job.ApplyUrl))
            throw new InternalJobRequiredException(request.JobId);

        await validator.ValidateAndThrowAsync(request);

        if (await applicationRepository.ExistsAsync(request.JobId, profile.Id))
            throw new AlreadyAppliedToJobException(request.JobId);

        var storageKey = $"job-applications/{request.JobId}/{Guid.NewGuid()}.pdf";

        var uploadRequest = new StorageUploadRequest(
            storageKey,
            request.CvContent,
            request.ContentType
        );

        await fileStoragePort.UploadAsync(uploadRequest);

        var application = new JobApplication(
            request.JobId,
            profile.Id,
            storageKey
        );

        try
        {
            await applicationRepository.CreateAsync(application);

            // Registrar interacción de postulación interna (peso 3.0 en modelo ALS)
            try
            {
                await interactionRepository.CreateAsync(
                    new JobInteraction(profile.CandidateProfile.ProfileId, request.JobId, JobInteractionType.InternalApply));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JobInteraction] Warning: Could not register InternalApply interaction: {ex.Message}");
            }

            return application.Id;
        }
        catch
        {
            await fileStoragePort.DeleteAsync(storageKey);
            throw;
        }
    }
}

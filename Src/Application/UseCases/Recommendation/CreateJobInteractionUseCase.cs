using Backend.Src.Application.Dtos.Requests.Recommendation;
using Backend.Src.Domain.Entities.Recommendation;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Repositories.Recommendation;
using Backend.Src.Domain.ValueObjects.Recommendation;

namespace Backend.Src.Application.UseCases.Recommendation;

public sealed class CreateJobInteractionUseCase(
    IJobInteractionRepository interactionRepository,
    IJobRepository jobRepository,
    IProfileRepository profileRepository)
{
    public async Task ExecuteAsync(CreateJobInteractionRequest request, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new InvalidOperationException("Candidate profile not found.");

        if (profile.CandidateProfile is null)
            throw new InvalidOperationException("Only candidate profiles can create job interactions.");

        var job = await jobRepository.GetByIdAsync(request.JobId)
            ?? throw new KeyNotFoundException("Job not found.");

        if (job.ClosesAt is not null && job.ClosesAt <= DateTime.UtcNow)
            throw new InvalidOperationException("Cannot interact with a closed job.");

        // Para eventos de visualización (View), aplicar un cooldown/debounce de 10 minutos para evitar duplicados por recarga de página
        if (request.Type == JobInteractionType.View)
        {
            if (await interactionRepository.ExistsRecentlyAsync(profile.CandidateProfile.ProfileId, request.JobId, request.Type, TimeSpan.FromMinutes(10)))
                return;
        }
        else
        {
            if (await interactionRepository.ExistsAsync(profile.CandidateProfile.ProfileId, request.JobId, request.Type))
                return;
        }

        await interactionRepository.CreateAsync(
            new JobInteraction(profile.CandidateProfile.ProfileId, request.JobId, request.Type));
    }
}

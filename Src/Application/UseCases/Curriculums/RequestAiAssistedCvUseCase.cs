using Backend.Src.Application.Dtos.Responses.Curriculums;
using Backend.Src.Application.UseCases.Payments;
using Backend.Src.Domain.Contracts.MessageBroker.Curriculums;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Rules.Curriculums;

namespace Backend.Src.Application.UseCases.Curriculums;

public class RequestAiAssistedCvUseCase(
    IMessageProducerPort messageProducer,
    IProfileRepository profileRepository,
    ICvRepository cvRepository,
    DeductCreditUseCase deductCreditUseCase,
    AddCreditsUseCase addCreditsUseCase
)
{
    public async Task<Guid> ExecuteAsync(Guid? jobId, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var cvId = Guid.NewGuid();
        var message = new AiAssistedCvGenerationMessage(cvId, jobId, userId);
        var now = DateTime.UtcNow;
        var cv = new Cv(
            cvId,
            profile.Id,
            $"CV_{profile.CandidateProfile.FirstName}_{now:yyyyMMdd_HHmmss}",
            false
        );
        cv.MarkProcessing();

        await cvRepository.CreateAsync(cv);
        try
        {
            await deductCreditUseCase.ExecuteAsync(userId, CvRules.CreditsPerAiGeneration);
        }
        catch
        {
            await cvRepository.DeleteAsync(cvId);
            throw;
        }

        try
        {
            await messageProducer.PublishAsync(message);
        }
        catch
        {
            await addCreditsUseCase.ExecuteAsync(userId, CvRules.CreditsPerAiGeneration);
            await cvRepository.DeleteAsync(cvId);
            throw;
        }

        return cvId;
    }
}

using Backend.Src.Application.Dtos.Requests.Curriculums;
using Backend.Src.Application.UseCases.Payments;
using Backend.Src.Domain.Contracts.MessageBroker.Curriculums;
using Backend.Src.Domain.Exceptions.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Rules.Curriculums;
using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Application.UseCases.Curriculums;

public class RequestAiAssistedCvImprovementUseCase(
    IMessageProducerPort messageProducer,
    IProfileRepository profileRepository,
    ICvRepository cvRepository,
    DeductCreditUseCase deductCreditUseCase,
    AddCreditsUseCase addCreditsUseCase
)
{
    public async Task ExecuteAsync(AiAssistedCvImprovementRequest request, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var cv = await cvRepository.GetByIdForUpdateAsync(request.CvId)
            ?? throw new CvNotFoundException(request.CvId);
        if (cv.CandidateId != profile.Id)
            throw new CvAccessDeniedException(cv.Id);
        if (cv.ProcessingStatus == CvProcessingStatus.Processing)
            throw new CvProcessingInProgressException(cv.Id);

        var message = new AiAssistedCvImprovementMessage(request.CvId, request.JobId, userId, request.Options);
        await deductCreditUseCase.ExecuteAsync(userId, CvRules.CreditsPerAiGeneration);
        try
        {
            cv.MarkProcessing();
            await cvRepository.UpdateAsync(cv);
            await messageProducer.PublishAsync(message);
        }
        catch
        {
            await addCreditsUseCase.ExecuteAsync(userId, CvRules.CreditsPerAiGeneration);
            cv.MarkReady();
            await cvRepository.UpdateAsync(cv);
            throw;
        }
    }
}

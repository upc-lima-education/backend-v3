using Backend.Src.Application.UseCases.Curriculums;
using Backend.Src.Application.UseCases.Payments;
using Backend.Src.Domain.Contracts.MessageBroker.Curriculums;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Rules.Curriculums;
using Backend.Src.Infrastructure.Persistence.PostgreSql;

namespace Backend.Src.Infrastructure.MessageBroker.Consumers.Curriculums;

public class CvAiAssistedImprovementConsumer(
    ProcessAiAssistedCvImprovementUseCase processAiAssistedCvImprovementUseCase,
    IServiceScopeFactory scopeFactory,
    ILogger<CvAiAssistedImprovementConsumer> logger
) : IMessageConsumerPort<AiAssistedCvImprovementMessage>
{
    public async Task ConsumeAsync(AiAssistedCvImprovementMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            await processAiAssistedCvImprovementUseCase.ExecuteAsync(message, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CV improvement failed for {CvId}", message.CvId);
            await using var recoveryScope = scopeFactory.CreateAsyncScope();
            var dbContext = recoveryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cvRepository = recoveryScope.ServiceProvider.GetRequiredService<ICvRepository>();
            var addCreditsUseCase = recoveryScope.ServiceProvider.GetRequiredService<AddCreditsUseCase>();
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var cv = await cvRepository.GetByIdForUpdateAsync(message.CvId);
            if (cv is not null)
            {
                cv.MarkFailed("No se pudo mejorar el CV. Se devolvió el crédito utilizado.");
                await cvRepository.UpdateAsync(cv);
            }
            await addCreditsUseCase.ExecuteAsync(message.UserId, CvRules.CreditsPerAiGeneration);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}

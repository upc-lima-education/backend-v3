using Backend.Src.Application.UseCases.Curriculums;
using Backend.Src.Application.UseCases.Payments;
using Backend.Src.Domain.Contracts.MessageBroker.Curriculums;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Rules.Curriculums;
using Backend.Src.Infrastructure.Persistence.PostgreSql;

namespace Backend.Src.Infrastructure.MessageBroker.Consumers.Curriculums;

public class CvAiAssistedGenerationConsumer(
    ProcessAiAssistedCvUseCase processAiAssistedCvUseCase,
    IServiceScopeFactory scopeFactory,
    ILogger<CvAiAssistedGenerationConsumer> logger
) : IMessageConsumerPort<AiAssistedCvGenerationMessage>
{
    public async Task ConsumeAsync(AiAssistedCvGenerationMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            await processAiAssistedCvUseCase.ExecuteAsync(message, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CV generation failed for {CvId}", message.CvId);
            await using var recoveryScope = scopeFactory.CreateAsyncScope();
            var dbContext = recoveryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cvRepository = recoveryScope.ServiceProvider.GetRequiredService<ICvRepository>();
            var addCreditsUseCase = recoveryScope.ServiceProvider.GetRequiredService<AddCreditsUseCase>();
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var cv = await cvRepository.GetByIdForUpdateAsync(message.CvId);
            if (cv is not null)
            {
                cv.MarkFailed("No se pudo generar el CV. Inténtalo nuevamente.");
                await cvRepository.UpdateAsync(cv);
            }
            await addCreditsUseCase.ExecuteAsync(message.UserId, CvRules.CreditsPerAiGeneration);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}

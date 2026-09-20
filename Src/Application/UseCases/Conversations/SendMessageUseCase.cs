using Backend.Src.Application.Dtos.Requests.Conversations;
using Backend.Src.Domain.Exceptions.Conversations;
using Backend.Src.Domain.Repositories.Conversations;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Conversations;

public class SendMessageUseCase(IConversationRepository conversationRepository, IValidator<SendMessageRequest> validator)
{
    public async Task ExecuteAsync(SendMessageRequest request, Guid senderId)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var conversation = await conversationRepository.GetByIdAsync(request.ConversationId)
            ?? throw new ConversationNotFoundException(request.ConversationId);

        conversation.SendMessage(senderId, request.Content);
        await conversationRepository.UpdateAsync(conversation);
    }
}

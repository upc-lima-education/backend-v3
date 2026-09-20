using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Conversations;

public sealed class ConversationAccessDeniedException(Guid conversationId)
    : DomainException($"You do not have access to conversation '{conversationId}'.")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Conversation access denied";
}
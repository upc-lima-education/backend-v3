using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Conversations;

public sealed class ConversationNotFoundException(Guid conversationId)
    : DomainException($"Conversation {conversationId} not found")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Conversation not found";
}
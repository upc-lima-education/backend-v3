using Backend.Src.Domain.Exceptions.Conversations;

namespace Backend.Src.Domain.Entities.Conversations;

public class Conversation
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid JobId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    private readonly List<ConversationMessage> _messages = [];
    public IReadOnlyCollection<ConversationMessage> Messages => _messages.AsReadOnly();

    private readonly List<ConversationUser> _users = [];
    public IReadOnlyCollection<ConversationUser> Users => _users.AsReadOnly();

    public Conversation() {}

    public Conversation(Guid jobId, List<Guid> users)
    {
        Id = Guid.NewGuid();
        JobId = jobId;
        CreatedAt = DateTime.UtcNow;
        _messages = [];
        _users = [];
        AddNewUsers(users);
    }

    public List<Guid> AddNewUsers(List<Guid> userIds)
    {
        var usersAdded = new List<Guid>();
        foreach (var userId in userIds)
        {
            if (_users.Any(u => u.UserId == userId)) continue;
            _users.Add(new ConversationUser(userId, Id));
            usersAdded.Add(userId);
        }
        return usersAdded;
    }

    public List<Guid> RemoveUsers(List<Guid> userIdsToRemove)
    {
        var usersToRemove = _users
            .Where(u => userIdsToRemove.Contains(u.UserId))
            .ToList();
        foreach (var user in usersToRemove)
        {
            _users.Remove(user);
        }
        return usersToRemove.Select(u => u.UserId).ToList();
    }

    public void SendMessage(Guid userId, string content)
    {
        if (!_users.Any(u => u.UserId == userId))
            throw new UserNotInConversationException(Id);
        _messages.Add(new ConversationMessage(Id, userId, content));
    }

    public static Conversation Restore(
        Guid id,
        Guid jobId,
        DateTime createdAt,
        IEnumerable<Guid> users,
        IEnumerable<ConversationMessage> messages
    )
    {
        var conversation = new Conversation
        {
            Id = id,
            JobId = jobId,
            CreatedAt = createdAt
        };

        foreach (var user in users)
            conversation._users.Add(new ConversationUser(user, id));

        conversation._messages.AddRange(messages);

        return conversation;
    }
}
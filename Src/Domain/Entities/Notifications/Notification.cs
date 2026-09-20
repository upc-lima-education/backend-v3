using Backend.Src.Domain.ValueObjects.Notifications;

namespace Backend.Src.Domain.Entities.Notifications;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    public Notification() {}

    public Notification(
        Guid userId,
        string message,
        NotificationType type
    )
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Message = message;
        Type = type;
        Status = NotificationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        SentAt = null;
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = NotificationStatus.Failed;
    }
}
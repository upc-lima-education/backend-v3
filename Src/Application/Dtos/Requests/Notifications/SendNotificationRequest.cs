using Backend.Src.Application.Dtos.Enums.Notifications;
using Backend.Src.Domain.ValueObjects.Notifications;

namespace Backend.Src.Application.Dtos.Requests.Notifications;

public record SendNotificationRequest(
    Guid ProfileId,
    NotificationType Type,
    List<NotificationChannel> Channels,
    string? Subject,
    string Message
);

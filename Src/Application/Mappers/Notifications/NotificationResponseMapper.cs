using Backend.Src.Application.Dtos.Responses.Notifications;
using Backend.Src.Domain.Entities.Notifications;

namespace Backend.Src.Application.Mappers.Notifications;

public static class NotificationResponseMapper
{
    public static NotificationResponse ToResponse (Notification notification)
    {
        return new NotificationResponse(
            notification.Id,
            notification.UserId,
            notification.Message,
            notification.Type.ToString(),
            notification.Status.ToString(),
            notification.CreatedAt,
            notification.SentAt
        );
    }
}
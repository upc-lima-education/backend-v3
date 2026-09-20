namespace Backend.Src.Application.Dtos.Responses.Notifications;

public record NotificationResponse(
    Guid Id,
    Guid UserId,
    string Message,
    string Type,
    string Status,
    DateTime CreatedAt,
    DateTime? SentAt
);

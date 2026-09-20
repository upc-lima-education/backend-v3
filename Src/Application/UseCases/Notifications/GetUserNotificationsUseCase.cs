using Backend.Src.Application.Dtos.Responses.Notifications;
using Backend.Src.Application.Mappers.Notifications;
using Backend.Src.Domain.Repositories.Notifications;

namespace Backend.Src.Application.UseCases.Notifications;

public class GetUserNotificationsUseCase(INotificationRepository repository)
{
    public async Task<IReadOnlyList<NotificationResponse>> ExecuteAsync(Guid userId)
    {
        var notifications = await repository.GetNotificationListByUserIdAsync(userId);
        var response = notifications.Select(NotificationResponseMapper.ToResponse).ToList();
        return response;
    }
}

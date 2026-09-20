using Backend.Src.Domain.Entities.Notifications;

namespace Backend.Src.Domain.Repositories.Notifications;

public interface INotificationRepository
{
    Task SaveAsync(Notification notification);
    Task<Notification?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Notification>> GetNotificationListByUserIdAsync(Guid userId);
}

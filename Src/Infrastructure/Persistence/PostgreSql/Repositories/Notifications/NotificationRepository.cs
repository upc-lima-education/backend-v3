using Microsoft.EntityFrameworkCore;
using Backend.Src.Domain.Repositories.Notifications;
using Backend.Src.Domain.Entities.Notifications;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Notifications;

public class NotificationRepository(AppDbContext context) : INotificationRepository
{
    public async Task SaveAsync(Notification notification)
    {
        var existing = await context.Notifications.FindAsync(notification.Id);
        
        if (existing is null) context.Notifications.Add(notification);
        else context.Notifications.Entry(existing).CurrentValues.SetValues(notification);

        await context.SaveChangesAsync();
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        var notification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        return notification;
    }

    public async Task<IReadOnlyList<Notification>> GetNotificationListByUserIdAsync(Guid userId)
    {
        var notificationList = await context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        
        return notificationList;
    }
}

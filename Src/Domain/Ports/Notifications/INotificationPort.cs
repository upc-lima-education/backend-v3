namespace Backend.Src.Domain.Ports.Notifications;

public interface INotificationPort
{
    Task<bool> SendAsync(string phoneNumber, string message);
}
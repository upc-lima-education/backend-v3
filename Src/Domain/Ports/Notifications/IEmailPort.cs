namespace Backend.Src.Domain.Ports.Notifications;

public interface IEmailPort
{
    Task<bool> SendPlainTextAsync(string receiver, string? subject, string body);
    Task<bool> SendHtmlAsync(string receiver, string? subject, string html);
}
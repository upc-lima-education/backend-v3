using Backend.Src.Domain.Ports.Notifications;
using Backend.Src.Infrastructure.Options.Notifications;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Backend.Src.Infrastructure.Adapters.Notifications;

public class EmailAdapter(IOptions<EmailOptions> options) : IEmailPort
{
    private readonly EmailOptions _options = options.Value;

    public async Task<bool> SendPlainTextAsync(string receiver, string? subject, string body)
    {
        return await SendEmailAsync(
            receiver,
            subject,
            new TextPart("plain") { Text = body }
        );
    }

    public async Task<bool> SendHtmlAsync(string receiver, string? subject, string html)
    {
        return await SendEmailAsync(
            receiver,
            subject,
            new TextPart("html") { Text = html }
        );
    }

    private async Task<bool> SendEmailAsync(string receiver, string? subject, TextPart body)
    {
        try
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_options.From));
            email.To.Add(MailboxAddress.Parse(receiver));
            email.Subject = subject ?? "Notification";
            email.Body = body;

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_options.Username, _options.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
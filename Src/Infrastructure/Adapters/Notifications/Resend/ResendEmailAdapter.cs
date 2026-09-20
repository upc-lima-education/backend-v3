using System.Net.Http.Json;
using Backend.Src.Domain.Ports.Notifications;
using Backend.Src.Infrastructure.Options.Notifications;
using Microsoft.Extensions.Options;

namespace Backend.Src.Infrastructure.Adapters.Notifications.Resend;

public class ResendEmailAdapter(HttpClient httpClient, IOptions<EmailOptions> options) : IEmailPort
{
    private readonly EmailOptions _options = options.Value;

    public async Task<bool> SendPlainTextAsync(string receiver, string? subject, string body)
    {
        return await SendViaApiAsync(receiver, subject, body, isHtml: false);
    }

    public async Task<bool> SendHtmlAsync(string receiver, string? subject, string html)
    {
        return await SendViaApiAsync(receiver, subject, html, isHtml: true);
    }

    private async Task<bool> SendViaApiAsync(string receiver, string? subject, string content, bool isHtml)
    {
        try
        {
            var payload = new
            {
                from = _options.From,
                to = new[] { receiver },
                subject = subject ?? "Notification",
                html = isHtml ? content : null,
                text = !isHtml ? content : null
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
            request.Headers.Add("Authorization", $"Bearer {_options.ApiKey}");
            request.Content = JsonContent.Create(payload);

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ResendError] Status: {response.StatusCode}, Body: {errorBody}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EmailError] Failed to send email via Resend API: {ex.Message}");
            return false;
        }
    }
}
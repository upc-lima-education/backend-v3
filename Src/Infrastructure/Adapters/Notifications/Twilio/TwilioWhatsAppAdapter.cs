using Backend.Src.Domain.Ports.Notifications;
using Backend.Src.Infrastructure.Options.Notifications;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Backend.Src.Infrastructure.Adapters.Notifications.Twilio;

public class TwilioWhatsAppAdapter(IOptions<TwilioOptions> options) : INotificationPort
{
    private readonly TwilioOptions _options = options.Value;

    public async Task<bool> SendAsync(string phoneNumber, string message)
    {
        try
        {
            TwilioClient.Init(_options.AccountSid, _options.AuthToken);

            var fromWhatsApp = _options.FromNumber;
            var toWhatsApp = $"whatsapp:{phoneNumber}";

            var result = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(fromWhatsApp),
                to: new PhoneNumber(toWhatsApp)
            );

            return result?.Sid != null;
        }
        catch
        {
            return false;
        }
    }
}
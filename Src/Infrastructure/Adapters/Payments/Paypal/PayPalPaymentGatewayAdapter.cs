using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Src.Domain.Contracts.Payments;
using Backend.Src.Domain.Ports.Payments;
using Backend.Src.Infrastructure.Contracts.Payments.Paypal;
using Backend.Src.Infrastructure.Options.Payments;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Backend.Src.Infrastructure.Adapters.Payments.Paypal;

public class PayPalPaymentGatewayAdapter(
    HttpClient http,
    IOptions<PayPalOptions> options,
    IMemoryCache cache
) : IPaymentGatewayPort
{
    private const string TokenCacheKey = "paypal:access_token";
    private readonly PayPalOptions _options = options.Value;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Creates an order <br/>
    /// See: https://developer.paypal.com/api/orders/v2/orders-create/
    /// </summary>
    public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        const string userAction = "PAY_NOW";
        await AuthorizeAsync();
        
        var payPalRequest = new PayPalCreateOrderRequest(
            Intent: "CAPTURE",
            PaymentSource: new PayPalPaymentSource(
                new PaypalSource(
                    new PayPalExperienceContext(
                        userAction,
                        request.ReturnUrl,
                        request.CancelUrl
                    )
                )
            ),
            PurchaseUnits: [
                new PayPalCreateOrderPurchaseUnit(
                    request.UserId.ToString(),
                    request.Description,
                    new PayPalAmount(
                        _options.Currency.Trim().ToUpperInvariant(),
                        request.Price.ToString("0.00", CultureInfo.InvariantCulture)
                    )
                )
            ]
        );

        using var payPalResponse = await http.PostAsJsonAsync("v2/checkout/orders", payPalRequest, JsonOptions);
        var responseBody = await payPalResponse.Content.ReadAsStringAsync();

        if (!payPalResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"PayPal order creation failed with {(int)payPalResponse.StatusCode} ({payPalResponse.StatusCode}). Body: {responseBody}"
            );
        }

        var order = await payPalResponse.Content.ReadFromJsonAsync<PayPalCreateOrderResponse>(JsonOptions)
                    ?? throw new InvalidOperationException("PayPal returned empty order");

        var approval = order.Links?
            .FirstOrDefault(l =>
                l.Rel.Equals("payer-action", StringComparison.OrdinalIgnoreCase) ||
                l.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase))
            ?.Href
            ?? throw new InvalidOperationException("PayPal did not return an approval URL");

        var response = new CreateOrderResponse(order.Id, approval);
        return response;
    }

    /// <summary>
    /// Captures payment for an order. To successfully capture payment for an order,
    /// the buyer must first approve the order. <br/>
    /// See: https://developer.paypal.com/api/orders/v2/orders-capture
    /// </summary>
    public async Task<string> CapturePaymentForOrderAsync(string orderId)
    {
        await AuthorizeAsync();

        using var content = new StringContent(""" {"payment_source": {}}""", Encoding.UTF8, "application/json");
        using var response = await http.PostAsync($"v2/checkout/orders/{orderId}/capture", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
        response.EnsureSuccessStatusCode();

        var capture = await response.Content.ReadFromJsonAsync<PayPalCaptureOrderResponse>(JsonOptions)
                      ?? throw new InvalidOperationException("PayPal returned empty capture");

        if (!string.Equals(capture.Status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Capture not completed. PayPal status code: {capture.Status}.");

        var transactionId = capture
            .PurchaseUnits.FirstOrDefault()?
            .Payments.Captures.FirstOrDefault()?
            .Id;

        return transactionId ?? throw new InvalidOperationException("PayPal did not returned a transactionId");
    }

    private async Task AuthorizeAsync()
    {
        var token = await cache.GetOrCreateAsync(TokenCacheKey, async entry =>
        {
            var fetched = await FetchTokenAsync();
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Math.Max(30, fetched.ExpiresIn - 60));
            return fetched.AccessToken;
        });

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// PayPal returns an access token and the number of seconds the access token is valid. <br/>
    /// See: https://developer.paypal.com/api/rest/authentication
    /// </summary>
    private async Task<PayPalTokenResponse> FetchTokenAsync()
    {
        var basic = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/oauth2/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PayPalTokenResponse>(JsonOptions)
               ?? throw new InvalidOperationException("PayPal did not returned a token");
    }
}

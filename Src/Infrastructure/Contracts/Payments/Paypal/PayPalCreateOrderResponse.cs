namespace Backend.Src.Infrastructure.Contracts.Payments.Paypal;

internal record PayPalCreateOrderResponse(
    string Id,
    string Status,
    IReadOnlyList<PayPalLink>? Links
);

internal record PayPalLink(
    string Href,
    string Rel,
    string? Method
);
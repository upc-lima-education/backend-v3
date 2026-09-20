namespace Backend.Src.Infrastructure.Contracts.Payments.Paypal;

public record PayPalAmount(
    string CurrencyCode,
    string Value
);
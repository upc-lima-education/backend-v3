namespace Backend.Src.Infrastructure.Contracts.Payments.Paypal;

internal record PayPalCreateOrderRequest(
    string Intent,
    PayPalPaymentSource PaymentSource,
    IReadOnlyList<PayPalCreateOrderPurchaseUnit> PurchaseUnits
);

public record PayPalPaymentSource(
    PaypalSource Paypal
);

public record PaypalSource(
    PayPalExperienceContext ExperienceContext
);

public record PayPalExperienceContext(
    string UserAction,
    string ReturnUrl,
    string CancelUrl
);

public record PayPalCreateOrderPurchaseUnit(
    string CustomId,
    string Description,
    PayPalAmount Amount
);
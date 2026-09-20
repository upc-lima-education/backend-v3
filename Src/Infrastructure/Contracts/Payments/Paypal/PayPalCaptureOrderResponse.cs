namespace Backend.Src.Infrastructure.Contracts.Payments.Paypal;

internal record PayPalCaptureOrderResponse(
    string Id,
    string Status,
    IReadOnlyList<PayPalCaptureOrderPurchaseUnit> PurchaseUnits
);

internal record PayPalCaptureOrderPurchaseUnit(
   PayPalCaptureOrderPayment Payments
);

internal record PayPalCaptureOrderPayment(
    IReadOnlyList<PayPalCaptureDetail> Captures
);

internal record PayPalCaptureDetail(
    string Id,
    string Status,
    PayPalAmount Amount
);
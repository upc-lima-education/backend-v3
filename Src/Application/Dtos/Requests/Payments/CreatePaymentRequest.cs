using Backend.Src.Domain.ValueObjects.Payments;

namespace Backend.Src.Application.Dtos.Requests.Payments;

public record CreatePaymentRequest(
    CreditPlan CreditPlan,
    PaymentPlatform Platform,
    string ReturnUrl,
    string CancelUrl
);
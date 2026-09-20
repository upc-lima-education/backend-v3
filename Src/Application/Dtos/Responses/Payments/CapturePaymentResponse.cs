namespace Backend.Src.Application.Dtos.Responses.Payments;

public record CapturePaymentResponse(
    bool Success,
    int CreditsAdded,
    int NewBalance,
    string? TransactionId
);
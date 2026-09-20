namespace Backend.Src.Application.Dtos.Responses.Payments;

public record CreatePaymentResponse(
    string OrderId,
    string ApprovalUrl
);
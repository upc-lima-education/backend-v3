namespace Backend.Src.Domain.Contracts.Payments;

public record CreateOrderResponse(
    string OrderId,
    string ApprovalUrl
);
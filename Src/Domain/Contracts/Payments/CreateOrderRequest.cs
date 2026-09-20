namespace Backend.Src.Domain.Contracts.Payments;

public record CreateOrderRequest(
    Guid UserId,
    string Description,
    decimal Price,
    string ReturnUrl,
    string CancelUrl
);
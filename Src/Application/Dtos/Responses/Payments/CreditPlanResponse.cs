namespace Backend.Src.Application.Dtos.Responses.Payments;

public record CreditPlanResponse(
    string Code,
    string Name,
    string Description,
    int Credits,
    decimal Price,
    string Currency,
    bool RequiresPayment
);

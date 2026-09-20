namespace Backend.Src.Domain.ValueObjects.Payments;

public record CreditPlanInfo(
    CreditPlan Plan,
    string Name,
    string Description,
    int Credits,
    decimal Price
);
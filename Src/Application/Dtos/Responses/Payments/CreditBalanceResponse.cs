namespace Backend.Src.Application.Dtos.Responses.Payments;

public record CreditBalanceResponse(
    int Balance,
    int InitialFreeCredits
);

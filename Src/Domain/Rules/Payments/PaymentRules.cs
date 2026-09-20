namespace Backend.Src.Domain.Rules.Payments;

public static class PaymentRules
{
    public const int MinAmountLength = 0;
    public const int MinCreditsPerPurchase = 1;
    public const int MaxCreditsPerPurchase = 100;
}
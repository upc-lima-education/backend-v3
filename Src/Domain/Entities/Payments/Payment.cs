using Backend.Src.Domain.ValueObjects.Payments;

namespace Backend.Src.Domain.Entities.Payments;

public class Payment
{
    public string OrderId { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public CreditPlan CreditPlan { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentPlatform Platform { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; private set; } = DateTime.UtcNow;

    public Payment() {}

    public Payment(
        string orderId,
        Guid userId,
        CreditPlan plan,
        PaymentPlatform platform
    )
    {
        OrderId = orderId;
        UserId = userId;
        CreditPlan = plan;
        Status = PaymentStatus.Pending;
        Platform = platform;
        CreatedAt = DateTime.UtcNow;
    }

    public void PaymentFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException();
        Status = PaymentStatus.Failed;
    }

    public void CompletePayment(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException();

        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        CompletedAt = DateTime.UtcNow;
    }

    public void CancelPayment()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException();

        Status = PaymentStatus.Cancelled;
    }
}

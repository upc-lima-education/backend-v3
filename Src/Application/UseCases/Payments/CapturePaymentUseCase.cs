using Backend.Src.Application.Dtos.Responses.Payments;
using Backend.Src.Domain.Exceptions.Payments;
using Backend.Src.Domain.Ports.Payments;
using Backend.Src.Domain.Repositories.Payments;
using Backend.Src.Domain.ValueObjects.Payments;

namespace Backend.Src.Application.UseCases.Payments;

public class CapturePaymentUseCase(
    IPaymentGatewayPort gateway,
    IPaymentRepository paymentRepository,
    AddCreditsUseCase addCreditsUseCase
)
{
    public async Task<CapturePaymentResponse> ExecuteAsync(string orderId, Guid userId)
    {
        var payment = await paymentRepository.GetByOrderIdAsync(orderId)
            ?? throw new PaymentNotFoundException(orderId);
        if (payment.UserId != userId)
            throw new PaymentOwnershipException();
        if (payment.Status == PaymentStatus.Completed)
            throw new PaymentAlreadyCompletedException(orderId);
        var transactionId = await gateway.CapturePaymentForOrderAsync(orderId);

        try
        {
            var plan = CreditPlanCatalog.Get(payment.CreditPlan);
            var newBalance = await addCreditsUseCase.ExecuteAsync(userId, plan.Credits);
            payment.CompletePayment(transactionId);
            await paymentRepository.SaveAsync(payment);

            return new CapturePaymentResponse(
                true,
                plan.Credits,
                newBalance,
                transactionId
            );
        }
        catch
        {
            payment.PaymentFailed();
            await paymentRepository.SaveAsync(payment);
            throw;
        }
    }
}
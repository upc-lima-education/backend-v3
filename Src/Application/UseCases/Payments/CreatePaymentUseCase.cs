using Backend.Src.Application.Dtos.Requests.Payments;
using Backend.Src.Application.Dtos.Responses.Payments;
using Backend.Src.Domain.Contracts.Payments;
using Backend.Src.Domain.Entities.Payments;
using Backend.Src.Domain.Ports.Payments;
using Backend.Src.Domain.Repositories.Payments;
using Backend.Src.Domain.ValueObjects.Payments;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Payments;

public class CreatePaymentUseCase(
    IPaymentGatewayPort gateway,
    IPaymentRepository paymentRepository,
    IValidator<CreatePaymentRequest> validator
)
{
    public async Task<CreatePaymentResponse> ExecuteAsync(Guid userId, CreatePaymentRequest request)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        
        var creditPlan = CreditPlanCatalog.Get(request.CreditPlan);

        var response = await gateway.CreateOrderAsync(
            new CreateOrderRequest(
                userId,
                $"{creditPlan.Name} Plan\n{creditPlan.Description}",
                creditPlan.Price,
                request.ReturnUrl,
                request.CancelUrl
            )
        );

        var payment = new Payment(
            response.OrderId,
            userId,
            request.CreditPlan,
            PaymentPlatform.Paypal
        );

        await paymentRepository.SaveAsync(payment);

        return new CreatePaymentResponse(
            response.OrderId,
            response.ApprovalUrl
        );
    }
}
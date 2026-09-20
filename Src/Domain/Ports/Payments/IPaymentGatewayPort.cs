using Backend.Src.Domain.Contracts.Payments;

namespace Backend.Src.Domain.Ports.Payments;

public interface IPaymentGatewayPort
{
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<string> CapturePaymentForOrderAsync(string orderId);
}
using Backend.Src.Domain.Entities.Payments;

namespace Backend.Src.Domain.Repositories.Payments;

public interface IPaymentRepository
{
    Task SaveAsync(Payment payment);
    Task<Payment?> GetByOrderIdAsync(string orderId);
}
using Backend.Src.Domain.Entities.Payments;
using Backend.Src.Domain.Repositories.Payments;
using Microsoft.EntityFrameworkCore;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Payments;

public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    public async Task SaveAsync(Payment payment)
    {
        var existing = await context.Payments.FindAsync(payment.OrderId);
        
        if (existing is null) context.Payments.Add(payment);
        else context.Entry(existing).CurrentValues.SetValues(payment);

        await context.SaveChangesAsync();
    }

    public async Task<Payment?> GetByOrderIdAsync(string orderId)
    {
        var payment = await context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
        return payment;
    }
}
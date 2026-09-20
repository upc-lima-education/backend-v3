using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Exceptions.Payments;
using Backend.Src.Domain.Repositories.Auth;

namespace Backend.Src.Application.UseCases.Payments;

public class DeductCreditUseCase(IUserRepository userRepository)
{
    public async Task<int> ExecuteAsync(Guid userId, int amount)
    {
        var user = await userRepository.GetByIdForUpdateAsync(userId)
            ?? throw new UserNotFoundException(userId);
        
        //Todo: Should be handle by fluent validation
        if (amount <= 0) throw new InvalidOperationException("Amount must be greater than 0");
        if (amount > user.CreditBalance)
            throw new InsufficientCreditsException(user.CreditBalance, amount);

        user.DeductCredits(amount);
        await userRepository.UpdateAsync(user);

        var response = user.CreditBalance;
        return response;
    }
}
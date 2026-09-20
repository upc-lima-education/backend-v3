using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Repositories.Auth;

namespace Backend.Src.Application.UseCases.Payments;

public class AddCreditsUseCase(IUserRepository userRepository)
{
    public async Task<int> ExecuteAsync(Guid userId, int amount)
    {
        var user = await userRepository.GetByIdForUpdateAsync(userId)
            ?? throw new UserNotFoundException(userId);

        //Todo: This should be handled by fluent validation
        if (amount <= 0) throw new InvalidOperationException("Amount must be greater than 0");

        user.AddCredits(amount);
        await userRepository.UpdateAsync(user);

        var response = user.CreditBalance;
        return response;
    }
}
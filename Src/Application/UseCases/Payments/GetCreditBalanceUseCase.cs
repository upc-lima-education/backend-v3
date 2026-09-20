using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Repositories.Auth;

namespace Backend.Src.Application.UseCases.Payments;

public class GetCreditBalanceUseCase(IUserRepository userRepository)
{
    public async Task<int> ExecuteAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new UserNotFoundException(userId);
        var response = user.CreditBalance;
        return response;
    }
}
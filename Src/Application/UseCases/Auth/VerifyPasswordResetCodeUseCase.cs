using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Ports.Auth;

namespace Backend.Src.Application.UseCases.Auth;

public class VerifyPasswordResetCodeUseCase(IPasswordResetTokenPort passwordResetTokenPort)
{
    public async Task<bool> ExecuteAsync(VerifyPasswordResetCodeRequest request)
    {
        var passwordData = await passwordResetTokenPort.GetAsync(request.Code)
            ?? throw new InvalidPasswordResetTokenException();
        await passwordResetTokenPort.MarkAsVerifiedAsync(request.Code);
        return true;
    }
}
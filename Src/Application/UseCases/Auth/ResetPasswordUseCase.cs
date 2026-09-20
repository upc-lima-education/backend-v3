using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Auth;

public class ResetPasswordUseCase(
    IUserRepository userRepository,
    IPasswordResetTokenPort passwordResetTokenPort,
    IPasswordHashPort passwordHashPort,
    IValidator<ResetPasswordRequest> validator
)
{
    public async Task ExecuteAsync(ResetPasswordRequest request)
    {
        var data = await passwordResetTokenPort.GetAsync(request.Code)
            ?? throw new InvalidPasswordResetTokenException();
        if (!data.IsVerified)
            throw new PasswordResetCodeNotVerifiedException();

        var user = await userRepository.GetByIdForUpdateAsync(data.UserId)
            ?? throw new UserNotFoundException(data.UserId);

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var passwordHash = passwordHashPort.HashPassword(request.NewPassword);
        user.ChangePassword(passwordHash);

        await userRepository.UpdateAsync(user);
        await passwordResetTokenPort.RemoveAsync(request.Code);
    }
}
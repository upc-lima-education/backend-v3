using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Auth;

public class ChangePasswordUseCase(
    IUserRepository userRepository,
    IPasswordHashPort passwordHashPort,
    IValidator<ChangePasswordRequest> validator
)
{
    public async Task ExecuteAsync(ChangePasswordRequest request, Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new UserNotFoundException(userId);

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var isCurrentPasswordValid = passwordHashPort.VerifyPassword(request.CurrentPassword, user.Password!);
        if (!isCurrentPasswordValid) throw new InvalidCurrentPasswordException();

        var passwordHash = passwordHashPort.HashPassword(request.NewPassword);
        user.ChangePassword(passwordHash);
        await userRepository.UpdateAsync(user);
    }
}
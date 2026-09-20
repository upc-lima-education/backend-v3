using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Auth;

public class SetPasswordUseCase(
    IUserRepository userRepository,
    IPasswordHashPort passwordHashPort,
    IValidator<SetPasswordRequest> validator
)
{
    public async Task ExecuteAsync(SetPasswordRequest request, Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new UserNotFoundException(userId);
        if (user.Password is not null) throw new PasswordAlreadySetException();

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var passwordHash = passwordHashPort.HashPassword(request.NewPassword);
        user.ChangePassword(passwordHash);
        await userRepository.UpdateAsync(user);
    }
}
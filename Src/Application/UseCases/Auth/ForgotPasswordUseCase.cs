using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Ports.Notifications;
using Backend.Src.Domain.Repositories.Auth;

namespace Backend.Src.Application.UseCases.Auth;

public class ForgotPasswordUseCase(
    IUserRepository userRepository,
    IPasswordResetTokenPort passwordResetTokenPort,
    IEmailPort emailPort,
    IHtmlTemplatePort htmlTemplatePort
)
{
    public async Task<bool> ExecuteAsync(ForgotPasswordRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        Console.WriteLine(user is null);
        if (user is null) return true;
        var code = await passwordResetTokenPort.GenerateAsync(user.Id, TimeSpan.FromMinutes(10));
        var html = htmlTemplatePort.Render(
            HtmlTemplate.PasswordReset,
            [
                ("CODE", code),
                ("EXPIRATION", "10 minutos")
            ]
        );
        await emailPort.SendHtmlAsync(
            user.Email,
            "Restablece tu contraseña",
            html
        );
        Console.WriteLine("Email sent");
        return true;
    }
}
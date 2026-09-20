namespace Backend.Src.Application.Dtos.Requests.Auth;

public record ResetPasswordRequest(
    string Code,
    string NewPassword
);
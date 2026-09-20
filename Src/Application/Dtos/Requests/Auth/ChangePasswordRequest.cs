namespace Backend.Src.Application.Dtos.Requests.Auth;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
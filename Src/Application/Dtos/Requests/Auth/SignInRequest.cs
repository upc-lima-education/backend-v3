namespace Backend.Src.Application.Dtos.Requests.Auth;

public record SignInRequest(
    string Email,
    string Password
);
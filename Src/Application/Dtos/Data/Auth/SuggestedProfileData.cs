namespace Backend.Src.Application.Dtos.Data.Auth;

public record SuggestedProfileData(
    string? FirstName,
    string? LastName,
    string? ProfilePicture
);
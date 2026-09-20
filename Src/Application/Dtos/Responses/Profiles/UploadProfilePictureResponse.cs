namespace Backend.Src.Application.Dtos.Responses.Profiles;

public record UploadProfilePictureResponse(
    Guid Id,
    Guid UserId,
    string? ProfilePicture,
    DateTime UpdatedAt
);
namespace Backend.Src.Application.Dtos.Requests.Profiles;

public record CreateCompanyProfileRequest(
    //Shared Data
    string? Description,
    string? Ubigeo,
    string? PhoneNumber,
    //CompanyData
    string CompanyName,
    string? Sector,
    string Ruc,
    string? Website,
    string? CompanySize,
    UploadProfilePictureRequest? ProfilePicture
);

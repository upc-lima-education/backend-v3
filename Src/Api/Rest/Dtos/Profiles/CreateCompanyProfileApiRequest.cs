namespace Backend.Src.Api.Rest.Dtos.Profiles;

public record CreateCompanyProfileApiRequest(
    //Shared Data
    string? Description,
    string? Ubigeo,
    string? PhoneNumber,
    List<string>? Skills,
    //Company Data
    string CompanyName,
    string? Sector,
    string Ruc,
    string? Website,
    string? CompanySize,
    IFormFile? ProfilePicture
);

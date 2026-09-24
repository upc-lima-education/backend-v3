namespace Backend.Src.Application.Dtos.Requests.Profiles;

public record UpdateCompanyProfileRequest(
    //Shared Data
    string? Description,
    string? Ubigeo,
    string? PhoneNumber,
    //CompanyData
    string? CompanyName,
    string? Sector,
    string? Website,
    string? CompanySize,
    string? Ruc = null
);

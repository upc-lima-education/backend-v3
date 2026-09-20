namespace Backend.Src.Api.Rest.Dtos.Profiles;

public record CreateCandidateProfileApiRequest(
    //Shared Data
    string? Description,
    string? Ubigeo,
    string? PhoneNumber,
    List<string>? Skills,
    //Candidate Data
    string FirstName,
    string LastName,
    string? Dni,
    IFormFile? ProfilePicture
);
namespace Backend.Src.Application.Dtos.Requests.Profiles;

public record UpdateCandidateProfileRequest(
    //Shared Data
    string? Description,
    string? Ubigeo,
    string? PhoneNumber,
    List<string>? Skills,
    //Candidate Data
    string? FirstName,
    string? LastName,
    string? Dni
);
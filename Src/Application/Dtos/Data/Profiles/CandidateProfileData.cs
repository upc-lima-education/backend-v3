namespace Backend.Src.Application.Dtos.Data.Profiles;

public record CandidateProfileData(
    string? FirstName,
    string? LastName,
    string? Dni = null
);
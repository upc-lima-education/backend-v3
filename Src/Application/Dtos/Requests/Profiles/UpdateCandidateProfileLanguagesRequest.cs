using Backend.Src.Application.Dtos.Data.Profiles;

namespace Backend.Src.Application.Dtos.Requests.Profiles;

public record UpdateCandidateProfileLanguagesRequest(List<LanguageKnownData> Languages);
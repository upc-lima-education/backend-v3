using Backend.Src.Application.Dtos.Data.Profiles;

namespace Backend.Src.Application.Dtos.Requests.Profiles;

public record UpdateCandidateProfileEducationsRequest(List<EducationData> Educations);
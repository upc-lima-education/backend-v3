using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Entities.Profiles;

namespace Backend.Src.Application.Mappers.Profiles;

public static class CandidateProfileMapper
{
    public static CandidateProfile ToEntity(CreateCandidateProfileRequest request, Profile profile)
    {
        return new CandidateProfile(
            profile.Id,
            profile,
            request.FirstName,
            request.LastName,
            request.Dni
        );
    }
}
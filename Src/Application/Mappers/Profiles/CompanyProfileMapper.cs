using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Entities.Profiles;

namespace Backend.Src.Application.Mappers.Profiles;

public static class CompanyProfileMapper
{
    public static CompanyProfile ToEntity(CreateCompanyProfileRequest request, Profile profile)
    {
        return new CompanyProfile(
            profile.Id,
            profile,
            request.CompanyName,
            request.Sector,
            request.Ruc,
            request.Website,
            request.CompanySize
        );
    }
}

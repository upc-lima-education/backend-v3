using Backend.Src.Application.Dtos.Data.Profiles;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Application.Dtos.Responses.Profiles;

namespace Backend.Src.Application.Mappers.Profiles;

public static class ProfileMapper
{
    public static ProfileResponse ToResponse(Profile profile)
    {
        var languages = profile.CandidateProfile?.Languages
            .Select(LanguageKnownMapper.ToData)
            .ToList() ?? [];

        var educations = profile.CandidateProfile?.Educations
            .Select(EducationMapper.ToData)
            .ToList() ?? [];

        var workExperiences = profile.CandidateProfile?.WorkExperiences
            .Select(WorkExperienceMapper.ToData)
            .ToList() ?? [];

        return new ProfileResponse(
            //Ids
            profile.Id,
            profile.UserId,
            //Shared data
            profile.Description,
            profile.Ubigeo,
            profile.Skills.Select(s => s.Name).ToList(),
            profile.ProfilePicture,
            profile.PhoneNumber,
            //Candidate data
            profile.CandidateProfile != null
                ? new CandidateProfileData(
                    profile.CandidateProfile.FirstName,
                    profile.CandidateProfile.LastName,
                    profile.CandidateProfile.Dni)
                : null,
            //Company data
            profile.CompanyProfile != null
                ? new CompanyProfileData(
                    profile.CompanyProfile.CompanyName,
                    profile.CompanyProfile.Sector,
                    profile.CompanyProfile.Ruc,
                    profile.CompanyProfile.Website,
                    profile.CompanyProfile.CompanySize,
                    profile.CompanyProfile.IsVerified)
                : null,
            // Candidate detailed lists
            languages,
            educations,
            workExperiences,
            //Validation
            profile.IsComplete(),
            //Traceability
            profile.CreatedAt,
            profile.UpdatedAt
        );
    }
}

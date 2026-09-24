using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Rules.Profiles;

namespace Backend.Src.Domain.Entities.Profiles;

public class CandidateProfile
{
    //Id
    public Guid ProfileId { get; private set; }
    public Profile Profile { get; private set; } = new Profile();
    //Details
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? Dni { get; private set; }
    private readonly List<LanguageKnown> _languages = [];
    public IReadOnlyCollection<LanguageKnown> Languages => _languages;
    private readonly List<WorkExperience> _workExperiences = [];
    public IReadOnlyCollection<WorkExperience> WorkExperiences => _workExperiences;
    private readonly List<Education> _educations = [];
    public IReadOnlyCollection<Education> Educations => _educations;
    //Traceability
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public CandidateProfile() {}

    public CandidateProfile
    (
        Guid profileId,
        Profile profile,
        string firstName,
        string lastName,
        string? dni
    )
    {
        ProfileId = profileId;
        Profile = profile;
        FirstName = firstName;
        LastName = lastName;
        Dni = string.IsNullOrWhiteSpace(dni) ? null : dni.Trim();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        string firstName,
        string lastName,
        string? dni
    )
    {
        FirstName = firstName;
        LastName = lastName;
        Dni = string.IsNullOrWhiteSpace(dni) ? null : dni.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLanguages(IReadOnlyCollection<LanguageKnown> languages)
    {
        if (languages.Count > CandidateProfileRules.MaxLanguagesKnown)
            throw new MaxLanguagesAchievedException(CandidateProfileRules.MaxLanguagesKnown);

        if (languages.GroupBy(language => language.LanguageCode).Any(group => group.Count() > 1))
            throw new DuplicateLanguageException();

        _languages.Clear();
        _languages.AddRange(languages);
    }

    public void UpdateWorkExperiences(List<WorkExperience> workExperiences)
    {
        if(workExperiences.Count > CandidateProfileRules.MaxWorkExperience)
            throw new MaxWorkExperienceAchievedException(CandidateProfileRules.MaxWorkExperience);
        
        _workExperiences.Clear();
        _workExperiences.AddRange(workExperiences);
    }

    public void UpdateEducations(List<Education> educations)
    {
        if (educations.Count > CandidateProfileRules.MaxEducationExperience)
            throw new MaxEducationAchievedException(CandidateProfileRules.MaxEducationExperience);
            
        _educations.Clear();
        _educations.AddRange(educations);
    }

}

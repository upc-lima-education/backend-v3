using Backend.Src.Domain.Entities.Skills;

namespace Backend.Src.Domain.Entities.Profiles;

public class Profile
{
    //Id
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    //Details
    public string? Description { get; private set; }
    public string? Ubigeo { get; private set; }
    public string? ProfilePicture { get; private set; }
    public string? PhoneNumber { get; private set; }
    private readonly List<Skill> _skills = [];
    public IReadOnlyCollection<Skill> Skills => _skills;
    //Traceability
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    //Profile types
    public CandidateProfile? CandidateProfile { get; set; }
    public CompanyProfile? CompanyProfile { get; set; }

    public Profile() {}

    public Profile(
        Guid userId,
        string? description,
        string? ubigeo,
        string? profilePicture,
        string? phoneNumber,
        List<Skill>? skills
    )
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Description = description;
        Ubigeo = ubigeo;
        ProfilePicture = profilePicture;
        PhoneNumber = phoneNumber;
        if(skills is not null) UpdateSkills(skills);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        string? description,
        string? ubigeo,
        string? phoneNumber,
        List<Skill>? skills
    )
    {
        Description = description;
        Ubigeo = ubigeo;
        PhoneNumber = phoneNumber;
        if (skills is not null) UpdateSkills(skills);
        UpdatedAt = DateTime.UtcNow;
    }

    public Profile UpdateProfilePicture(string profilePicture)
    {
        ProfilePicture = profilePicture;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public void UpdateSkills(List<Skill> skills)
    {
        _skills.Clear();
        _skills.AddRange(skills);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsComplete()
    {
        if (string.IsNullOrEmpty(Description))
            return false;

        if (CandidateProfile is not null)
        {
            return
                !string.IsNullOrEmpty(CandidateProfile.FirstName)
                && !string.IsNullOrEmpty(CandidateProfile.LastName);
        }

        if (CompanyProfile is not null)
        {
            return !string.IsNullOrEmpty(CompanyProfile.CompanyName);
        }

        return false;
    }
}
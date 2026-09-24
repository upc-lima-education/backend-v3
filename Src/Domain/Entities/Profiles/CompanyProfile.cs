namespace Backend.Src.Domain.Entities.Profiles;

public class CompanyProfile
{
    //Id
    public Guid ProfileId { get; private set; }
    public Profile Profile { get; private set; } = new Profile();
    //Details
    public string CompanyName { get; private set; } = string.Empty;
    public string? Sector { get; private set; }
    public string? Ruc { get; private set; }
    public string? Website { get; private set; }
    public string? CompanySize { get; private set; }
    //Verification
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public Guid? VerifiedByUserId { get; private set; }
    //Traceability
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public CompanyProfile() {}

    public CompanyProfile(
        Guid profileId,
        Profile profile,
        string companyName,
        string? sector,
        string? ruc,
        string? website,
        string? companySize
    )
    {
        ProfileId = profileId;
        Profile = profile;
        CompanyName = companyName;
        Sector = sector;
        Ruc = string.IsNullOrWhiteSpace(ruc) ? null : ruc.Trim();
        Website = website;
        CompanySize = companySize;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update
    (
        string companyName,
        string? sector,
        string? website,
        string? companySize,
        string? ruc = null
    )
    {
        CompanyName = companyName;
        Sector = sector;
        Website = website;
        CompanySize = companySize;

        var cleanRuc = string.IsNullOrWhiteSpace(ruc) ? null : ruc.Trim();
        if (Ruc != cleanRuc)
        {
            Ruc = cleanRuc;
            IsVerified = false;
            VerifiedAt = null;
            VerifiedByUserId = null;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyCompany(Guid userId)
    {
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
        VerifiedByUserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}

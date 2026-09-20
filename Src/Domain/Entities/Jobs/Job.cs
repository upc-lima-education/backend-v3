using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Entities.Skills;
using Backend.Src.Domain.ValueObjects.Jobs;

namespace Backend.Src.Domain.Entities.Jobs;

public class Job
{
    // Ids
    public Guid Id { get; private set; }
    public Guid? CompanyId { get; private set; }
    public CompanyProfile? Company { get; private set; }
    // Details
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public JobType JobType { get; private set; }
    public WorkHours WorkHours { get; private set; }
    // Requirements
    private readonly List<Skill> _skills = [];
    public IReadOnlyCollection<Skill> Skills => _skills;
    public Experience Experience { get; private set; }
    public EducationLevel EducationLevel { get; private set; }
    // Location
    public string? Ubigeo { get; private set; }
    public string? Address { get; private set; }
    // Payment
    public decimal? MinSalary { get; private set; }
    public decimal? MaxSalary { get; private set; }
    public Currency? Currency { get; private set; }
    public SalaryPeriod? SalaryPeriod { get; private set; }
    public CompensationType? CompensationType { get; private set; }
    // Traceability
    public DateTime OpensAt { get; private set; }
    public DateTime? ClosesAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public JobStatus JobStatus => CalculateStatus();
    public OriginPage OriginPage { get; private set; }
    public int Views { get; private set; }
    // External
    public string? ExternalCompanyName { get; private set; }
    public string? ExternalCompanyImage { get; private set; }
    public string? SourceUrl { get; private set; }
    public string? ApplyUrl { get; private set; }

    public Job() {}

    public Job(
        //Id
        Guid? companyId,
        //Details
        string title,
        string description,
        JobType jobType,
        WorkHours workHours,
        // Requirements
        List<Skill> skills,
        Experience experience,
        EducationLevel educationLevel,
        //Location
        string? ubigeo,
        string? address,
        //Payment
        decimal? minSalary,
        decimal? maxSalary,
        Currency? currency,
        SalaryPeriod? salaryPeriod,
        CompensationType? compensationType,
        // Traceability
        DateTime opensAt,
        DateTime? closesAt,
        OriginPage originPage,
        //External
        string? externalCompanyName,
        string? sourceUrl,
        string? applyUrl,
        string? externalCompanyImage = null
    )
    {
        //Id
        Id = Guid.NewGuid();
        CompanyId = companyId;
        //Details
        Title = title;
        Description = description;
        JobType = jobType;
        WorkHours = workHours;
        //Requirements
        UpdateJobSkills(skills);
        Experience = experience;
        EducationLevel = educationLevel;
        //Location
        Ubigeo = ubigeo;
        Address = address;
        //Payment
        MinSalary = minSalary;
        MaxSalary = maxSalary;
        Currency = currency;
        SalaryPeriod = salaryPeriod;
        CompensationType = compensationType;
        //Traceability
        OpensAt = opensAt;
        ClosesAt = closesAt;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        OriginPage = originPage;
        Views = 0;
        //External
        ExternalCompanyName = externalCompanyName;
        ExternalCompanyImage = externalCompanyImage;
        SourceUrl = sourceUrl;
        ApplyUrl = applyUrl;
    }

    public void Update(
        //Details
        string title,
        string description,
        JobType jobType,
        WorkHours workHours,
        //Requirements
        List<Skill> skills,
        Experience experience,
        EducationLevel educationLevel,
        //Location
        string? ubigeo,
        string? address,
        //Payment
        decimal? minSalary,
        decimal? maxSalary,
        Currency? currency,
        SalaryPeriod? salaryPeriod,
        CompensationType? compensationType,
        //Traceability
        DateTime opensAt,
        DateTime closesAt,
        string? applyUrl
    )
    {
        //Details
        Title = title;
        Description = description;
        JobType = jobType;
        WorkHours = workHours;
        //Requirements
        UpdateJobSkills(skills);
        Experience = experience;
        EducationLevel = educationLevel;
        //Location
        Ubigeo = ubigeo;
        Address = address;
        //Payment
        MinSalary = minSalary;
        MaxSalary = maxSalary;
        Currency = currency;
        SalaryPeriod = salaryPeriod;
        CompensationType = compensationType;
        //Traceability
        OpensAt = opensAt;
        ClosesAt = closesAt;
        UpdatedAt = DateTime.UtcNow;
        //Applications
        ApplyUrl = applyUrl;
    }

    public void UpdateSchedule(DateTime opensAt, DateTime closesAt)
    {
        OpensAt = opensAt;
        ClosesAt = closesAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateJobSkills(List<Skill> skills)
    {
        _skills.Clear();
        _skills.AddRange(skills);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClaimJob(CompanyProfile companyProfile)
    {
        Company = companyProfile;
        CompanyId = companyProfile.ProfileId;
        SourceUrl = "";
        ExternalCompanyName = "";
        ExternalCompanyImage = "";
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssociateCompany(CompanyProfile companyProfile)
    {
        Company = companyProfile;
        CompanyId = companyProfile.ProfileId;
        if (!string.IsNullOrWhiteSpace(companyProfile.CompanyName))
        {
            ExternalCompanyName = companyProfile.CompanyName;
        }
        var picture = companyProfile.Profile?.ProfilePicture;
        if (!string.IsNullOrWhiteSpace(picture))
        {
            ExternalCompanyImage = picture;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsOwnedBy(Guid profileId) => Company?.ProfileId == profileId;

    private JobStatus CalculateStatus()
    {
        var now = DateTime.UtcNow;
        if (now < OpensAt) return JobStatus.Scheduled;
        if (now >= ClosesAt) return JobStatus.Closed;
        return JobStatus.Active;
    }
}

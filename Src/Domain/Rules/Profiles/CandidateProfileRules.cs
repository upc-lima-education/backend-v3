namespace Backend.Src.Domain.Rules.Profiles;

public static class CandidateProfileRules
{
    public const int MinNameLenth = 2;
    public const int MaxNameLength = 64;
    public const int MaxLanguagesKnown = 50;
    //Work Experience
    public const int MaxWorkExperience = 15;
    public const int MaxCompanyLength = 128;
    public const int MaxPositionLength = 64;
    public const int MaxDescriptionLength = 2000;
    //Education
    public const int MaxEducationExperience = 15;
    public const int MaxInstitutionLength = 128;
    public const int MaxDegreeLength = 64;
    public const int MaxFieldOfStudyLength = 64;
}
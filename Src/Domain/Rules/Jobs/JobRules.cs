namespace Backend.Src.Domain.Rules.Jobs;

public static class JobRules
{
    //Title
    public const int MinTitleLength = 3;
    public const int MaxTitleLength = 255;

    //Description
    public const int MinDescriptionLength = 10;
    public const int MaxDescriptionLength = 5000;
    
    //Address
    public const int MaxAddressLength = 255;
    
    //Ubigeo
    public const short UbigeoLength = 6;

    //Skills
    public const int MaxSkillsAmount = 20;
}
namespace Backend.Src.Domain.Rules.Profiles;

public static class ProfileRules
{
    public const int MaxDescriptionLength = 2048;
    public const int MaxProfilePictureSize = 2 * 1024 * 1024; //2MB
    public static readonly string[] AllowedProfilePictureExtensions = ["image/png", "image/jpg", "image/jpeg"];
}
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Rules.Profiles;
using FluentValidation;

namespace Backend.Src.Application.Validators.Profiles;

public class UploadProfilePictureValidator : AbstractValidator<UploadProfilePictureRequest>
{
    private const int MaxProfilePictureSize = ProfileRules.MaxProfilePictureSize;
    private static readonly string[] AllowedProfilePictureExtensions = ProfileRules.AllowedProfilePictureExtensions;
    public UploadProfilePictureValidator()
    {
        RuleFor(x => x.File.Content.Length)
            .LessThanOrEqualTo(MaxProfilePictureSize).WithMessage($"Image must be at most {MaxProfilePictureSize} bytes");
        
        RuleFor(x => x.File.ContentType)
            .Must(ext =>AllowedProfilePictureExtensions.Contains(ext.ToLowerInvariant())).WithMessage("Only PNG and JPG images are allowed");
    }
}
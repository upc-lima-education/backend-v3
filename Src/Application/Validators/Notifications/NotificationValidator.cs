using Backend.Src.Application.Dtos.Requests.Notifications;
using Backend.Src.Domain.Rules.Notifications;
using FluentValidation;

namespace Backend.Src.Application.Validators.Notifications;

internal sealed class NotificationValidator : AbstractValidator<SendNotificationRequest>
{
    private const int MinMessageLength = NotificationRules.MinMessageLength;
    private const int MaxMessageLength = NotificationRules.MaxMessageLength;

    public NotificationValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Notification message cannot be empty")
            .MinimumLength(MinMessageLength).WithMessage($"Notification message must be at least {MinMessageLength} characters long")
            .MaximumLength(MaxMessageLength).WithMessage($"Notification message must be most {MaxMessageLength} characters long");
    }
}
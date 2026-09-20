using Backend.Src.Application.Dtos.Enums.Notifications;
using Backend.Src.Application.Dtos.Requests.Notifications;
using Backend.Src.Application.Dtos.Responses.Notifications;
using Backend.Src.Application.Mappers.Notifications;
using Backend.Src.Domain.Entities.Notifications;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Notifications;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Domain.Repositories.Notifications;
using Backend.Src.Domain.Repositories.Profiles;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Notifications;

public class SendNotificationUseCase(
    INotificationRepository notificationRepository,
    INotificationPort notificationService,
    IEmailPort emailService,
    IProfileRepository profileRepository,
    IUserRepository userRepository,
    IValidator<SendNotificationRequest> validator
)
{
    public async Task<NotificationResponse> ExecuteAsync(SendNotificationRequest request)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var profile = await profileRepository.GetByIdAsync(request.ProfileId)
            ?? throw new ProfileNotFoundException(request.ProfileId);

        var notification = new Notification(
            profile.UserId,
            request.Message,
            request.Type
        );

        await notificationRepository.SaveAsync(notification);
        var isMessageSent = false;

        if (request.Channels.Contains(NotificationChannel.WhatsApp))
        {
            if (profile.PhoneNumber is null)
                throw new PhoneNumberRequiredException(profile.Id);
            isMessageSent |= await notificationService.SendAsync(profile.PhoneNumber, request.Message);
        }

        if (request.Channels.Contains(NotificationChannel.Email))
        {
            var user = await userRepository.GetByIdAsync(profile.UserId)
                ?? throw new UserNotFoundException(profile.UserId);
            isMessageSent |= await emailService.SendPlainTextAsync(user.Email, request.Subject, request.Message);
        }

        if (isMessageSent) notification.MarkAsSent();
        else notification.MarkAsFailed();

        await notificationRepository.SaveAsync(notification);

        var response = NotificationResponseMapper.ToResponse(notification);
        return response;
    }
}
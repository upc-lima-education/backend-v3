using Backend.Src.Application.Dtos.Enums.Notifications;
using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Contracts.MessageBroker.Recruitment;
using Backend.Src.Domain.Entities.Notifications;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Domain.Ports.Notifications;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Domain.Repositories.Notifications;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Notifications;

namespace Backend.Src.Infrastructure.MessageBroker.Consumers.Recruitment;

public class ApproveJobApplicationConsumer(
    INotificationRepository notificationRepository,
    IProfileRepository profileRepository,
    IUserRepository userRepository,
    INotificationPort notificationPort,
    IEmailPort emailPort,
    IHtmlTemplatePort htmlTemplatePort
) : IMessageConsumerPort<ApproveJobApplicationMessage>
{
    public async Task ConsumeAsync(ApproveJobApplicationMessage message, CancellationToken cancellationToken = default)
    {
        var profile = await profileRepository.GetByIdAsync(message.CandidateId)
            ?? throw new ProfileNotFoundException(message.CandidateId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var user = await userRepository.GetByIdAsync(profile.UserId)
            ?? throw new UserNotFoundException(profile.UserId);

        var notification = new Notification(
            profile.UserId,
            $"Tu postulación al empleo \"{message.JobTitle}\" en \"{message.CompanyName}\" ha sido aceptada.",
            NotificationType.ApplicationAccepted
        );

        await notificationRepository.SaveAsync(notification);

        var sent = false;

        if (message.Channels.Contains(NotificationChannel.Email))
        {
            var html = htmlTemplatePort.Render(
                HtmlTemplate.CandidateSelected,
                [
                    ("CANDIDATE_NAME", profile.CandidateProfile.FirstName),
                    ("COMPANY_NAME", message.CompanyName),
                    ("JOB_TITLE", message.JobTitle)
                ]
            );
            sent |= await emailPort.SendHtmlAsync(
                user.Email,
                $"Actualización de postulación al empleo: {message.JobTitle}",
                html
            );
        }

        if (message.Channels.Contains(NotificationChannel.WhatsApp)
            && !string.IsNullOrWhiteSpace(profile.PhoneNumber))
        {
            sent |= await notificationPort.SendAsync(
                profile.PhoneNumber,
                $"Hola {profile.CandidateProfile.FirstName} 👋\n\n" +
                "Tenemos una actualización sobre tu postulación al puesto " +
                $"*{message.JobTitle}* en *{message.CompanyName}*\n\n" +
                "✅ La empresa ha decidido continuar contigo en el proceso de selección.\n\n" +
                "📌 Te recomendamos estar atento a los medios de contacto registrados en tu cuenta.\n\n" +
                "_Este es un mensaje automático de LLanqui. Por favor, no respondas a este número._"
            );
        }

        if (sent) notification.MarkAsSent();
        else notification.MarkAsFailed();

        await notificationRepository.SaveAsync(notification);
    }
}
using Backend.Src.Application.Dtos.Enums.Notifications;

namespace Backend.Src.Application.Dtos.Requests.Recruitment;
public record ApproveJobApplicationRequest(List<NotificationChannel> Channels);
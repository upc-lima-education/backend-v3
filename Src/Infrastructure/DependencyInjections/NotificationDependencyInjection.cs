using Backend.Src.Application.Dtos.Requests.Notifications;
using Backend.Src.Application.UseCases.Notifications;
using Backend.Src.Application.Validators.Notifications;
using Backend.Src.Domain.Ports.Notifications;
using Backend.Src.Domain.Repositories.Notifications;
using Backend.Src.Infrastructure.Adapters.Notifications;
using Backend.Src.Infrastructure.Adapters.Notifications.Twilio;
using Backend.Src.Infrastructure.Options.Notifications;
using Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Notifications;
using FluentValidation;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class NotificationDependencyInjection
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();

        //Configuration
        services.Configure<TwilioOptions>(configuration.GetSection("Twilio"));
        services.Configure<EmailOptions>(configuration.GetSection("Email"));

        // Adapters        
        services.AddScoped<INotificationPort, TwilioWhatsAppAdapter>();
        services.AddScoped<IEmailPort, EmailAdapter>();

        //Fluent Validation
        services.AddScoped<IValidator<SendNotificationRequest>, NotificationValidator>();

        // Use cases
        services.AddScoped<SendNotificationUseCase>();
        services.AddScoped<GetUserNotificationsUseCase>();

        return services;
    }
}

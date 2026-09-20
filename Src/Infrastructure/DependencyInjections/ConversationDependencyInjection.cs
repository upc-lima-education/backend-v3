using Backend.Src.Application.Dtos.Requests.Conversations;
using Backend.Src.Application.UseCases.Conversations;
using Backend.Src.Domain.Repositories.Conversations;
using Backend.Src.Infrastructure.Persistence.MongoDb.Repositories.Conversations;
using FluentValidation;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class ConversationDependencyInjection
{
    public static IServiceCollection AddConversationModule(this IServiceCollection services)
    {
        //Repository
        services.AddScoped<IConversationRepository, ConversationRepository>();

        //FluentValidation
        services.AddScoped<IValidator<SendMessageRequest>, SendMessageValidator>();

        //Uses Cases
        services.AddScoped<CreateConversationUseCase>();
        services.AddScoped<DeleteConversationUseCase>();
        services.AddScoped<SendMessageUseCase>();
        services.AddScoped<AddUsersToConversationUseCase>();
        services.AddScoped<RemoveUsersFromConversationUseCase>();
        services.AddScoped<GetConversationByIdUseCase>();
        services.AddScoped<GetConversationListByJobIdUseCase>();
        services.AddScoped<GetMyConversationsUseCase>();

        return services;
    }
}

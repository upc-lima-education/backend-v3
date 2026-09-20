using Backend.Src.Application.UseCases.Recruitment;
using Backend.Src.Application.Dtos.Requests.Recruitment;
using Backend.Src.Application.Validators.Recruitment;
using Backend.Src.Domain.Contracts.MessageBroker.Recruitment;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Domain.Repositories.Recruitment;
using Backend.Src.Infrastructure.MessageBroker.Consumers.Recruitment;
using Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Recruitment;
using FluentValidation;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class RecruitmentDependencyInjection
{
    public static IServiceCollection AddRecruitmentModule(this IServiceCollection services)
    {
        //Repository
        services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

        // Fluent Validation
        services.AddScoped<IValidator<CreateJobApplicationRequest>, CreateJobApplicationValidator>();

        //Use cases
        services.AddScoped<CreateJobApplicationUseCase>();
        services.AddScoped<GetJobApplicationUseCase>();
        services.AddScoped<GetJobApplicationsByJobUseCase>();
        services.AddScoped<ApproveJobApplicationUseCase>();
        services.AddScoped<RejectJobApplicationUseCase>();
        services.AddScoped<GetMyJobApplicationsUseCase>();

        return services;
    }
}

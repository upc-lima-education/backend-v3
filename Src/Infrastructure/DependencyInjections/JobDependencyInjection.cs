using Backend.Src.Application.Dtos.Requests.Jobs;
using Backend.Src.Application.Validators.Jobs;
using Backend.Src.Application.UseCases.Jobs;
using Backend.Src.Application.UseCases.Recommendation;
using Backend.Src.Application.Dtos.Requests.Recommendation;
using Backend.Src.Application.Validators.Recommendation;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Recommendation;
using Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Jobs;
using Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Recommendation;
using Backend.Src.Infrastructure.Adapters.Recommendation;
using Backend.Src.Infrastructure.Options.Recommendation;
using FluentValidation;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class JobDependencyInjection
{
    public static IServiceCollection AddJobsModule(this IServiceCollection services, IConfiguration configuration)
    {
        //Repositories
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IJobInteractionRepository, JobInteractionRepository>();
        services.AddScoped<CreateJobInteractionUseCase>();
        services.AddScoped<GetRecommendationsForCandidateUseCase>();
        services.AddScoped<SearchRecommendationsUseCase>();
        services.AddScoped<IValidator<SearchRecommendationsRequest>, SearchRecommendationsValidator>();
        services.Configure<RecommendationOptions>(
            configuration.GetSection("Recommendation"));
        services.AddHttpClient<IRecommendationClient, RecommendationClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<RecommendationOptions>>().Value;
            if (!string.IsNullOrWhiteSpace(options.BaseUrl)) client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
        });

        //Fluent Validation
        services.AddScoped<IValidator<CreateInternalJobRequest>, CreateInternalJobValidator>();
        services.AddScoped<IValidator<UpdateJobRequest>, UpdateJobValidator>();
        services.AddScoped<IValidator<PatchJobScheduleRequest>, PatchJobScheduleValidator>();
        services.AddScoped<IValidator<PatchJobSkillsRequest>, PatchJobSkillsValidator>();

        //Use cases
        services.AddScoped<ClaimJobUseCase>();
        services.AddScoped<CreateInternalJobUseCase>();
        services.AddScoped<DeleteJobUseCase>();
        services.AddScoped<GetJobByIdUseCase>();
        services.AddScoped<GetJobListUseCase>();
        services.AddScoped<GetJobSummaryUseCase>();
        services.AddScoped<GetJobListByCompanyIdUseCase>();
        services.AddScoped<PatchJobScheduleUseCase>();
        services.AddScoped<PatchJobSkillsUseCase>();
        services.AddScoped<SyncScrapedJobsUseCase>();
        services.AddScoped<UpdateJobUseCase>();

        return services;
    }
}

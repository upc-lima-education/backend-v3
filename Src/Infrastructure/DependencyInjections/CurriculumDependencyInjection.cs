using System.Net.Http.Headers;
using Backend.Src.Application.Dtos.Requests.Curriculums;
using Backend.Src.Application.UseCases.Curriculums;
using Backend.Src.Application.Validators.Curriculums;
using Backend.Src.Domain.Ports.Curriculums;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Infrastructure.Adapters.Curriculums;
using Backend.Src.Infrastructure.Adapters.Curriculums.OpenRouter;
using Backend.Src.Infrastructure.Options.Curriculums;
using Backend.Src.Infrastructure.Persistence.MongoDb.Repositories.Curriculums;
using Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Curriculums;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class CurriculumDependencyInjection
{
    public static IServiceCollection AddCvModule(this IServiceCollection services, IConfiguration configuration)
    {
        //Configuration
        services.Configure<OpenRouterOptions>(configuration.GetSection("OpenRouter"));

        //Repositories
        services.AddScoped<ICvRepository, CvRepository>();
        services.AddScoped<ICvStructuredContentRepository, CvStructuredContentRepository>();

        //Adapters
        services.AddScoped<ICvHtmlRendererPort, CvHtmlRendererAdapter>();
        services.AddHttpClient<ICvAiAssistantPort, OpenRouterCvAiAssistantAdapter>(
            (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<OpenRouterOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromMinutes(5);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);

                client.DefaultRequestHeaders.Add("HTTP-Referer", options.HttpReferer);
                client.DefaultRequestHeaders.Add("X-Title", options.Title);
            }
        );

        //Fluent Validation
        services.AddScoped<IValidator<CreateCvStructuredContentRequest>, CreateCvStructuredContentValidator>();
        services.AddScoped<IValidator<CreateCvUploadedContentRequest>, CreateCvUploadedContentValidator>();
        
        //Use cases
        services.AddScoped<CreateCvStructuredContentUseCase>();
        services.AddScoped<CreateCvUploadedContentUseCase>();
        services.AddScoped<GetStructuredCvUseCase>();
        services.AddScoped<GetCvUploadedContentUseCase>();
        services.AddScoped<DeleteCvUseCase>();
        services.AddScoped<RequestAiAssistedCvUseCase>();
        services.AddScoped<ProcessAiAssistedCvUseCase>();
        services.AddScoped<RequestAiAssistedCvImprovementUseCase>();
        services.AddScoped<ProcessAiAssistedCvImprovementUseCase>();
        services.AddScoped<GenerateCvPdfUseCase>();
        services.AddScoped<GetMyCvsUseCase>();
        services.AddScoped<GetCvProcessingStatusUseCase>();
        
        return services;
    }
}

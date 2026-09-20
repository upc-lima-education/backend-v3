using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.UseCases.Profiles;
using Backend.Src.Application.Validators.Profiles;
using Backend.Src.Domain.Ports.Profiles;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Infrastructure.Adapters.Profiles.Decolecta;
using Backend.Src.Infrastructure.Options.Profiles;
using Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Profiles;
using FluentValidation;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class ProfileDependencyInjection
{
    public static IServiceCollection AddProfilesModule(this IServiceCollection services, IConfiguration configuration)
    {
        //Repositories
        services.AddScoped<IProfileRepository, ProfileRepository>();

        //Configuration
        services.Configure<DecolectaOptions>(configuration.GetSection("Decolecta"));

        //Adapters
        services.AddScoped<IRucValidationPort, DecolectaSunatRucValidatorAdapter>();

        //Fluent Validation
        services.AddScoped<IValidator<CreateCandidateProfileRequest>, CreateCandidateProfileValidator>();
        services.AddScoped<IValidator<CreateCompanyProfileRequest>, CreateCompanyProfileValidator>();
        services.AddScoped<IValidator<UpdateCandidateProfileRequest>, UpdateCandidateProfileValidator>();
        services.AddScoped<IValidator<UpdateCompanyProfileRequest>, UpdateCompanyProfileValidator>();
        services.AddScoped<IValidator<ValidateRucRequest>, ValidateRucValidator>();
        services.AddScoped<IValidator<UploadProfilePictureRequest>, UploadProfilePictureValidator>();
        services.AddScoped<IValidator<UpdateCandidateProfileEducationsRequest>, UpdateCandidateProfileEducationsValidator>();
        services.AddScoped<IValidator<UpdateCandidateProfileWorkExperiencesRequest>, UpdateCandidateProfileWorkExperiencesValidator>();

        //Use cases
        services.AddScoped<ExternalCreateProfileUseCase>();
        services.AddScoped<CreateCandidateProfileUseCase>();
        services.AddScoped<CreateOrganizationProfileUseCase>();
        services.AddScoped<GetProfileByProfileIdUseCase>();
        services.AddScoped<GetMyProfileUseCase>();
        services.AddScoped<UpdateCandidateProfileUseCase>();
        services.AddScoped<UpdateCompanyProfileUseCase>();
        services.AddScoped<UploadProfilePictureUseCase>();
        services.AddScoped<ValidateRucUseCase>();
        services.AddScoped<VerifyCompanyProfileUseCase>();
        services.AddScoped<UpdateCandidateProfileLanguagesUseCase>();
        services.AddScoped<UpdateCandidateProfileEducationsUseCase>();
        services.AddScoped<UpdateCandidateProfileWorkExperiencesUseCase>();
        return services;
    }
}

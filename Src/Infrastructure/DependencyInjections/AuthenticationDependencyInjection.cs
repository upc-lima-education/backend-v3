using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.UseCases.Auth;
using Backend.Src.Application.Validators.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Infrastructure.Adapters.Auth;
using Backend.Src.Infrastructure.Adapters.Auth.OAuth;
using Backend.Src.Infrastructure.Options.Auth;
using Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Auth;
using Backend.Src.Infrastructure.Persistence.Redis.Repositories.Auth;
using FluentValidation;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class AuthenticationDependencyInjection
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {
        //Configuration
        services.Configure<JwtOptions>(configuration.GetSection("JWT"));
        services.Configure<GoogleAuthOptions>(configuration.GetSection("Google"));

        //Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        //Adapters
        services.AddSingleton<IJwtPort, JwtAdapter>();
        services.AddScoped<IPasswordHashPort, PasswordHashAdapter>();
        services.AddScoped<IPasswordResetTokenPort, PasswordResetTokenAdapter>();
        services.AddHttpClient<IExternalAuthenticationPort, GoogleAuthenticationAdapter>();

        //Fluent Validation
        services.AddScoped<IValidator<SignUpRequest>, SignUpValidator>();
        services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordValidator>();
        services.AddScoped<IValidator<RefreshTokenRequest>, RefreshTokenValidator>();
        services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordValidator>();
        services.AddScoped<IValidator<UpdateContactRequest>, UpdateContactValidator>();

        //Use Cases
        //Authentication use cases
        services.AddScoped<SignUpUseCase>();
        services.AddScoped<SignInUseCase>();
        services.AddScoped<GetCurrentUserUseCase>();
        //External authentication use cases
        services.AddScoped<ExternalAuthenticationUseCase>();
        services.AddScoped<ExternalGetAuthenticationUrlUseCase>();
        services.AddScoped<ExternalSignInUseCase>();
        services.AddScoped<ExternalSignUpUseCase>();
        //Password use cases
        services.AddScoped<ResetPasswordUseCase>();
        services.AddScoped<ForgotPasswordUseCase>();
        services.AddScoped<ChangePasswordUseCase>();
        services.AddScoped<UpdateContactUseCase>();
        services.AddScoped<VerifyPasswordResetCodeUseCase>();
        //Refresh Token
        services.AddScoped<RefreshTokenUseCase>();
        

        return services;
    }
}

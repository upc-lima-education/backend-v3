using System.Text;
using System.Text.Json.Serialization;
using Backend.Src.Api.Rest.Filters.Jobs;
using Backend.Src.Infrastructure.Options.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Backend.Src.Api.DependencyInjections;

public static class ApiDependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Llanqui API",
                    Version = "v1",
                    Description = "API REST"
                }
            );

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                }
            );

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        AddAuthentication(services, configuration);
        AddCors(services, configuration);

        services.AddScoped<ScraperApiKeyFilter>();

        return services;
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                {
                    var jwtSecret = configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("JWT:Secret not configured");
                    var jwtIssuer = configuration["JWT:Issuer"] ?? "llanqui-api";
                    var jwtAudience = configuration["JWT:Audience"] ?? "llanqui-api";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        ClockSkew = TimeSpan.Zero
                    };
                }
            );

        services.AddAuthorization();
    }

    private static void AddCors(IServiceCollection services, IConfiguration configuration)
    {
        var googleSettings = configuration.GetSection("Google").Get<GoogleAuthOptions>()
            ?? throw new InvalidOperationException("Google Settings not configured");

        services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy.SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}
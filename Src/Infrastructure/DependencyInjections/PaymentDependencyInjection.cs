using Backend.Src.Application.UseCases.Payments;
using Backend.Src.Domain.Ports.Payments;
using Backend.Src.Application.Dtos.Requests.Payments;
using Backend.Src.Application.Validators.Payments;
using Backend.Src.Domain.Repositories.Payments;
using Backend.Src.Infrastructure.Adapters.Payments.Paypal;
using Backend.Src.Infrastructure.Options.Payments;
using Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Payments;
using FluentValidation;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class PaymentsDependencyInjection
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        //Options
        services.Configure<PayPalOptions>(configuration.GetSection("Paypal"));

        //Repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        //Paypal
        var payPal = configuration.GetSection("Paypal").Get<PayPalOptions>() ?? throw new InvalidOperationException("Paypal not configured");
        services.AddMemoryCache();
        services.AddHttpClient<IPaymentGatewayPort, PayPalPaymentGatewayAdapter>(client =>
            {
                client.BaseAddress = new Uri(payPal.BaseUrl + "/");
                client.Timeout     = TimeSpan.FromSeconds(30);
            }
        ).AddResilienceHandler("paypal", builder =>
            {
                builder.AddRetry(
                    new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        BackoffType      = DelayBackoffType.Exponential,
                        Delay            = TimeSpan.FromSeconds(2),
                        UseJitter        = false,
                        ShouldHandle     = args => ValueTask.FromResult(args.Outcome.Result is { } resp && ((int)resp.StatusCode >= 500 || (int)resp.StatusCode == 429))
                    }
                );
            }
        );

        // Validation and supporting use cases
        services.AddScoped<IValidator<CreatePaymentRequest>, CreatePaymentValidator>();
        services.AddScoped<AddCreditsUseCase>();
        services.AddScoped<DeductCreditUseCase>();
        services.AddScoped<GetCreditBalanceUseCase>();

        // Use cases
        services.AddScoped<CreatePaymentUseCase>();
        services.AddScoped<CapturePaymentUseCase>();

        return services;
    }
}

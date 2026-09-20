using Backend.Src.Domain.Contracts.MessageBroker.Curriculums;
using Backend.Src.Domain.Contracts.MessageBroker.Recruitment;
using Backend.Src.Domain.Ports.MessageBroker;
using Backend.Src.Infrastructure.MessageBroker.Consumers.Curriculums;
using Backend.Src.Infrastructure.MessageBroker.Consumers.Recruitment;
using Backend.Src.Infrastructure.MessageBroker.RabbitMQ;
using Backend.Src.Infrastructure.Options.Common;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class MessageBrokerDependencyInjection {
    public static IServiceCollection AddMessageBrokerModule(this IServiceCollection services, IConfiguration configuration)
    {
        //Configuration
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.PostConfigure<RabbitMqOptions>(opt =>
        {
            var connStr = configuration.GetConnectionString("RabbitMQ");
            if (!string.IsNullOrWhiteSpace(connStr) && connStr.StartsWith("amqp", StringComparison.OrdinalIgnoreCase))
            {
                opt.Uri = connStr;
            }
        });

        //Producers
        services.AddScoped<IMessageProducerPort, RabbitMqProducerAdapter>();
        
        // Workers
        services.AddHostedService<RabbitMqMessageWorker<AiAssistedCvGenerationMessage>>();
        services.AddHostedService<RabbitMqMessageWorker<AiAssistedCvImprovementMessage>>();
        services.AddHostedService<RabbitMqMessageWorker<ApproveJobApplicationMessage>>();
        services.AddHostedService<RabbitMqMessageWorker<RejectJobApplicationMessage>>();

        //Consumers
        services.AddScoped<IMessageConsumerPort<AiAssistedCvGenerationMessage>, CvAiAssistedGenerationConsumer>();
        services.AddScoped<IMessageConsumerPort<AiAssistedCvImprovementMessage>, CvAiAssistedImprovementConsumer>();
        services.AddScoped<IMessageConsumerPort<ApproveJobApplicationMessage>, ApproveJobApplicationConsumer>();
        services.AddScoped<IMessageConsumerPort<RejectJobApplicationMessage>, RejectJobApplicationConsumer>();
        //RabbitMQ
        services.AddSingleton<IConnection>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            try
            {
                ConnectionFactory factory;
                if (!string.IsNullOrWhiteSpace(options.Uri))
                {
                    factory = new ConnectionFactory
                    {
                        Uri = new Uri(options.Uri),
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
                    };
                }
                else
                {
                    factory = new ConnectionFactory
                    {
                        HostName = options.Host,
                        Port = options.Port,
                        UserName = options.Username,
                        Password = options.Password,
                        VirtualHost = options.VirtualHost,
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(3)
                    };
                }

                return factory.CreateConnectionAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ] Warning: Could not connect to message broker: {ex.Message}");
                return null;
            }
        });
        return services;
    }
}
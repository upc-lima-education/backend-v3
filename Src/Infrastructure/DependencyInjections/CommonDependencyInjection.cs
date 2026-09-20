using Backend.Src.Application.Resolvers.Common;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Repositories.Common;
using Backend.Src.Infrastructure.Adapters.Common;
using Backend.Src.Infrastructure.Options.Common;
using Backend.Src.Infrastructure.Persistence.Json;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class CommonDependencyInjection {
    public static IServiceCollection AddCommonModule(this IServiceCollection services, IConfiguration configuration)
    {
        //Adapters
        var cvStorageSection = configuration.GetSection("CvStorage");
        var storageSection = configuration.GetSection("Storage");
        var provider = cvStorageSection["Provider"] ?? storageSection["Provider"] ?? "Local";

        if (string.Equals(provider, "AzureBlob", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = cvStorageSection["AzureConnectionString"] 
                ?? storageSection["AzureConnectionString"] 
                ?? string.Empty;
            var containerName = cvStorageSection["AzureContainerName"] 
                ?? storageSection["AzureContainerName"] 
                ?? "cvs";
            services.AddSingleton(new Azure.Storage.Blobs.BlobContainerClient(connectionString, containerName));
            services.AddScoped<IFileStoragePort, AzureBlobStorageAdapter>();
        }
        else
        {
            services.AddScoped<IFileStoragePort, LocalDiskStorageAdapter>();
        }

        services.AddScoped<IHtmlTemplatePort, HtmlTemplateAdapter>();
        services.AddScoped<IPdfRendererPort, PlaywrightPdfRendererAdapter>();
        //Repositories
        services.AddScoped<IUbigeoRepository, UbigeoRepository>();
        //Resolver
        services.AddScoped<UbigeoResolver>();

        //Configuration
        services.Configure<LocalBlobStorageOptions>(configuration.GetSection("Storage"));
        return services;
    }
}
using Backend.Src.Infrastructure.Persistence.MongoDb;
using Backend.Src.Infrastructure.Persistence.PostgreSql;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class PersistenceDependencyInjection
{
    public static IServiceCollection AddPersistenceModule(this IServiceCollection services, IConfiguration configuration)
    {
        //PostgreSQL
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        //MongoDB
        try
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch (BsonSerializationException) { }

        var mongoConfiguration = configuration.GetConnectionString("MongoDbConnection");
        var mongoClient = new MongoClient(mongoConfiguration);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("Lima-Db"));
        services.AddSingleton<MongoDbContext>();

        //Redis
        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
        redisOptions.AbortOnConnectFail = false;
        redisOptions.ConnectTimeout = 3000;
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));
        return services;
    }
}

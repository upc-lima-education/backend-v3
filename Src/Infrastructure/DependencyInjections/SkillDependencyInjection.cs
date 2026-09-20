using Backend.Src.Application.UseCases.Skills;
using Backend.Src.Domain.Repositories.Skills;
using Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Skills;
using Backend.Src.Application.Resolvers.Skills;

namespace Backend.Src.Infrastructure.DependencyInjections;

public static class SkillDependencyInjection
{
    public static IServiceCollection AddSkillsModule(this IServiceCollection services)
    {
        //Resolver
        services.AddScoped<SkillResolver>();

        //Repository
        services.AddScoped<ISkillRepository, SkillRepository>();

        //Use cases
        services.AddScoped<GetSkillListUseCase>();
        
        return services;
    }
}

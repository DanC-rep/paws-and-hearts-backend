using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;
using PawsAndHearts.VolunteerRequests.Infrastructure.Repositories;

namespace PawsAndHearts.VolunteerRequests.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddVolunteerRequestsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDatabase()
            .AddRepositories();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Modules.VolunteerRequests);

        services.AddScoped<WriteDbContext>();

        services.AddScoped<IVolunteerRequestsReadDbContext, ReadDbContext>();
        
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IVolunteerRequestsRepository, VolunteerRequestsRepository>();

        services.AddScoped<IUserRestrictionRepository, UserRestrictionRepository>();
        
        return services;
    }
}
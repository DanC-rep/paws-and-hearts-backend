using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.Discussions.Infrastructure.DbContexts;
using PawsAndHearts.Discussions.Infrastructure.Repositories;

namespace PawsAndHearts.Discussions.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddDiscussionsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabase()
            .AddRepositories();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Modules.Discussions);

        services.AddScoped<WriteDbContext>();

        services.AddScoped<IDiscussionsReadDbContext, ReadDbContext>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDiscussionsRepository, DiscussionsRepository>();

        return services;
    }
}
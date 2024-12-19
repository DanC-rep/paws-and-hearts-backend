using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Options;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Infrastructure.BackgroundServices;
using PawsAndHearts.PetManagement.Infrastructure.DbContexts;
using PawsAndHearts.PetManagement.Infrastructure.Repositories;
using PawsAndHearts.PetManagement.Infrastructure.Services;

namespace PawsAndHearts.PetManagement.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddPetManagementInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SoftDeleteOptions>(
            configuration.GetSection(SoftDeleteOptions.SOFT_DELETE));
        
        services
            .AddDbContexts(configuration)
            .AddRepositories()
            .AddDatabase()
            .AddBackgroundServices();

        return services;
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddScoped<DeleteExpiredVolunteersService>();
        services.AddScoped<DeleteExpiredPetsService>();
        services.AddHostedService<DeleteExpiredEntitiesBackgroundService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Modules.PetManagement);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IVolunteersRepository, VolunteersRepository>();

        return services;
    }

    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<WriteDbContext>(_ =>
            new WriteDbContext(configuration.GetConnectionString(Constants.DATABASE)!));
        
        services.AddScoped<IVolunteersReadDbContext, ReadDbContext>(_ =>
            new ReadDbContext(configuration.GetConnectionString(Constants.DATABASE)!));

        return services;
    }
}
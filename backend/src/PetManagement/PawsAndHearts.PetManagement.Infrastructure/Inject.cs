using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Files;
using PawsAndHearts.Core.Messaging;
using PawsAndHearts.Core.Options;
using PawsAndHearts.Framework.BackgroundServices;
using PawsAndHearts.PetManagement.Application;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Infrastructure.BackgroundServices;
using PawsAndHearts.PetManagement.Infrastructure.DbContexts;
using PawsAndHearts.PetManagement.Infrastructure.Options;
using PawsAndHearts.PetManagement.Infrastructure.Providers;
using PawsAndHearts.PetManagement.Infrastructure.Repositories;
using PawsAndHearts.PetManagement.Infrastructure.Services;
using PawsAndHearts.SharedKernel.Interfaces;
using FileInfo = PawsAndHearts.SharedKernel.FileProvider.FileInfo;

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
            .AddMinio(configuration)
            .AddRepositories()
            .AddDatabase()
            .AddBackgroundServices();

        services.AddSingleton<IMessageQueue<IEnumerable<FileInfo>>,
            FilesCleanerMessageQueue<IEnumerable<FileInfo>>>();
        
        return services;
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddScoped<IFilesCleanerService, FilesCleanerService>();
        services.AddHostedService<FilesCleanerBackgroundService>();

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

    private static IServiceCollection AddMinio(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMinio(options =>
        {
            var minioOptions = configuration.GetSection(MinioOptions.MINIO).Get<MinioOptions>()
                               ?? throw new ApplicationException("Missing minio configuration");

            options.WithEndpoint(minioOptions.Endpoint);
            options.WithCredentials(minioOptions.AccessKey, minioOptions.AccessKey);
            options.WithSSL(minioOptions.WithSsl);
        });

        services.AddScoped<IFileProvider, MinioProvider>();

        return services;
    }

}
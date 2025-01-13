using System.Reflection;
using Amazon.S3;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using FileService.Data.Options;
using FileService.Infrastructure.MongoDataAccess;
using FileService.Infrastructure.Providers;
using FileService.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Minio;
using MongoDB.Driver;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Events;

namespace FileService;

public static class DependencyInjection
{
    public static IServiceCollection AddFileServiceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddLogging(configuration)
            .AddMinio(configuration)
            .AddAmazonS3()
            .AddMongoDb(configuration)
            .AddHangfire(configuration)
            .AddRepositories()
            .AddMetrics(configuration);

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
    
    private static IServiceCollection AddLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string indexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:dd-MM-yyyy}";
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Elasticsearch(
                [new Uri(configuration.GetConnectionString("Elasticsearch")!)],
                options =>
                {
                    options.DataStream = new DataStreamName(indexFormat);
                    options.TextFormatting = new EcsTextFormatterConfiguration();
                    options.BootstrapMethod = BootstrapMethod.Silent;
                })
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .CreateLogger();

        services.AddSerilog();

        return services;
    }

    private static IServiceCollection AddAmazonS3(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = "http://localhost:9000",
                ForcePathStyle = true
            };
    
            return new AmazonS3Client("minioadmin", "minioadmin", config);
        });

        return services;
    }

    private static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(
            new MongoClient(configuration.GetConnectionString("Mongo")));

        services.AddScoped<FileMongoDbContext>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IFilesRepository, FilesRepository>();

        return services;
    }

    private static IServiceCollection AddHangfire(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(configuration.GetConnectionString("Database")))
        );

        services.AddHangfireServer();

        return services;
    }
    
    private static IServiceCollection AddMetrics(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(b => b
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("FileService"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddPrometheusExporter());

        return services;
    }
}
using System.Reflection;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PawsAndHearts.Accounts.Application;
using PawsAndHearts.Accounts.Application.Consumers;
using PawsAndHearts.Accounts.Infrastructure;
using PawsAndHearts.Accounts.Presentation;
using PawsAndHearts.BreedManagement.Infrastructure;
using PawsAndHearts.BreedManagement.Presentation;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Options;
using PawsAndHearts.Discussions.Infrastructure;
using PawsAndHearts.Discussions.Presentation;
using PawsAndHearts.Framework.Authorization;
using PawsAndHearts.PetManagement.Infrastructure;
using PawsAndHearts.PetManagement.Presentation;
using PawsAndHearts.VolunteerRequests.Application;
using PawsAndHearts.VolunteerRequests.Infrastructure;
using Serilog;
using Serilog.Events;

namespace PawsAndHearts.Web;

public static class Inject
{
    public static IServiceCollection AddModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddCustomSwaggerGen()
            .AddAccountsModule(configuration)
            .AddBreedManagementModule(configuration)
            .AddPetManagementModule(configuration)
            .AddDiscussionsModule(configuration)
            .AddVolunteerRequestsModule(configuration)
            .AddApplicationLayers()
            .AddAuthServices(configuration)
            .AddMessageBus(configuration)
            .AddMetrics(configuration);

        return services;
    }
    
    private static IServiceCollection AddAuthServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>()
                    ?? throw new ApplicationException("Missing jwt configuration");

                options.TokenValidationParameters = TokenValidationParametersFactory
                    .CreateWithLifeTime(jwtOptions);
            });

        services.AddAuthorization();

        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        
        services.AddHttpContextAccessor()
            .AddScoped<UserScopedData>();

        return services;
    }

    private static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "My API",
                Version = "v1"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddCustomOptions(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SoftDeleteOptions>(
            configuration.GetSection(SoftDeleteOptions.SOFT_DELETE));

        return services;
    }

    public static IServiceCollection AddLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string indexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:dd-MM-yyyy}";
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Elasticsearch(
                [new Uri("http://localhost:9200")],
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

    private static IServiceCollection AddMetrics(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(b => b
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PawsAndHearts.Web"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddPrometheusExporter());

        return services;
    }

    private static IServiceCollection AddAccountsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAccountsInfrastructure(configuration)
            .AddAccountsApplication(configuration)
            .AddAccountsPresentation();

        return services;
    }

    private static IServiceCollection AddBreedManagementModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddBreedManagementInfrastructure(configuration)
            .AddBreedManagementPresentation();

        return services;
    }

    private static IServiceCollection AddPetManagementModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddPetManagementInfrastructure(configuration)
            .AddPetManagementPresentation();

        return services;
    }

    private static IServiceCollection AddDiscussionsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDiscussionsInfrastructure(configuration)
            .AddDiscussionsPresentation();

        return services;
    }
    
    private static IServiceCollection AddVolunteerRequestsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddVolunteerRequestsInfrastructure(configuration)
            .AddVolunteerRequestsApplication();

        return services;
    }
    
    private static IServiceCollection AddApplicationLayers(this IServiceCollection services)
    {
        var assemblies = new[]
        {
            typeof(PawsAndHearts.Accounts.Application.Inject).Assembly,
            typeof(PawsAndHearts.BreedManagement.Application.Inject).Assembly,
            typeof(PawsAndHearts.Discussions.Application.Inject).Assembly,
            typeof(PawsAndHearts.PetManagement.Application.Inject).Assembly,
            typeof(PawsAndHearts.VolunteerRequests.Application.Inject).Assembly
        };

        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes
                .AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes
                .AssignableToAny(typeof(IQueryHandler<,>), typeof(IQueryHandlerWithResult<,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssemblies(assemblies);
        return services;
    }

    private static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            configure.AddConsumer<ApproveVolunteerRequestEventConsumer>();

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["RabbitMQ:Host"]!), h =>
                {
                    h.Username(configuration["RabbitMQ:Username"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });

                cfg.Durable = true;

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
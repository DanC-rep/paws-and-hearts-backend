using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;
using PawsAndHearts.VolunteerRequests.Infrastructure.Outbox;
using PawsAndHearts.VolunteerRequests.Infrastructure.Repositories;
using Quartz;

namespace PawsAndHearts.VolunteerRequests.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddVolunteerRequestsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddQuartzServices();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Modules.VolunteerRequests);

        services.AddScoped<WriteDbContext>(_ =>
            new WriteDbContext(configuration.GetConnectionString(Constants.DATABASE)!));

        services.AddScoped<IVolunteerRequestsReadDbContext, ReadDbContext>(_ =>
            new ReadDbContext(configuration.GetConnectionString(Constants.DATABASE)!));
        
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IVolunteerRequestsRepository, VolunteerRequestsRepository>();

        services.AddScoped<IUserRestrictionRepository, UserRestrictionRepository>();

        services.AddScoped<IOutboxRepository, OutboxRepository>();
        
        return services;
    }

    private static IServiceCollection AddQuartzServices(this IServiceCollection services)
    {
        services.AddScoped<ProcessOutboxMessageService>();

        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessageJob));

            configure
                .AddJob<ProcessOutboxMessageJob>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey).WithSimpleSchedule(
                    schedule => schedule.WithIntervalInSeconds(1).RepeatForever()));
        });

        services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });

        return services;
    }
}
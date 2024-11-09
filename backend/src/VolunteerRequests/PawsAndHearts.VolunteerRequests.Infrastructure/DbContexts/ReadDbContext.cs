using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Contracts.Dtos;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;

public class ReadDbContext(IConfiguration configuration) : DbContext, IVolunteerRequestsReadDbContext
{
    public IQueryable<VolunteerRequestDto> VolunteerRequests => Set<VolunteerRequestDto>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(Constants.DATABASE));
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(WriteDbContext).Assembly,
            type => type.FullName?.Contains("Configurations.Read") ?? false);

        modelBuilder.HasDefaultSchema("volunteer-requests");
    }
    
    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => builder.AddConsole());
}
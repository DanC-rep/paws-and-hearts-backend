using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawsAndHearts.VolunteerRequests.Domain.Entities;
using PawsAndHearts.VolunteerRequests.Infrastructure.Outbox;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;

public class WriteDbContext : DbContext
{
    private readonly string _connectionString;

    public WriteDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    public DbSet<VolunteerRequest> VolunteerRequests => Set<VolunteerRequest>();
    public DbSet<UserRestriction> UserRestrictions => Set<UserRestriction>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(WriteDbContext).Assembly,
            type => type.FullName?.Contains("Configurations.Write") ?? false);
        
        modelBuilder.HasDefaultSchema("volunteer-requests");
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => builder.AddConsole());
}
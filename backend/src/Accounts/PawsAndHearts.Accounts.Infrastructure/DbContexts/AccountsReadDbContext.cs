using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Contracts.Dtos;

namespace PawsAndHearts.Accounts.Infrastructure.DbContexts;

public class AccountsReadDbContext(IConfiguration configuration) : DbContext, IReadDbContext
{
    public IQueryable<UserDto> Users => Set<UserDto>();
    
    public IQueryable<RoleDto> Roles => Set<RoleDto>();
    
    public IQueryable<AdminAccountDto> AdminAccounts => Set<AdminAccountDto>();
    
    public IQueryable<ParticipantAccountDto> ParticipantAccounts => Set<ParticipantAccountDto>();
    
    public IQueryable<VolunteerAccountDto> VolunteerAccounts => Set<VolunteerAccountDto>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(Constants.DATABASE));
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("accounts");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AccountsReadDbContext).Assembly,
            type => type.FullName?.Contains("Configuration.Read") ?? false);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Accounts.Domain;

namespace PawsAndHearts.Accounts.Infrastructure.DbContexts;

public class AccountsWriteDbContext(IConfiguration configuration) 
    : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Permission> Permissions => Set<Permission>();
    
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    
    public DbSet<ParticipantAccount> ParticipantAccounts => Set<ParticipantAccount>();
    
    public DbSet<VolunteerAccount> VolunteerAccounts => Set<VolunteerAccount>();
    
    public DbSet<AdminAccount> AdminAccounts => Set<AdminAccount>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(Constants.DATABASE));
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Role>()
            .ToTable("roles");
        
        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("user_claims");
        
        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("user_tokens");
        
        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("user_logins");
        
        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("role_claims");
        
        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("user_roles");
        
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AccountsWriteDbContext).Assembly,
            type => type.FullName?.Contains("Configuration.Write") ?? false);

        modelBuilder.HasDefaultSchema("accounts");
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => builder.AddConsole());
}
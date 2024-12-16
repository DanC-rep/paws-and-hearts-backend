using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Accounts.Infrastructure.DbContexts;
using PawsAndHearts.Accounts.Infrastructure.IdentityManagers;
using PawsAndHearts.Accounts.Infrastructure.Providers;
using PawsAndHearts.Accounts.Infrastructure.Seeding;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Options;

namespace PawsAndHearts.Accounts.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureCustomOptions(configuration);

        services.AddDatabase(configuration);
        
        services.AddScoped<ITokenProvider, JwtTokenProvider>();   
        
        services.RegisterIdentity();

        services.AddSingleton<AccountSeeder>();

        services.AddScoped<AccountsSeederService>();

        services.AddIdentityManagers();
        
        return services;
    }

    private static IServiceCollection RegisterIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<AccountsWriteDbContext>();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AccountsWriteDbContext>(_ =>
            new AccountsWriteDbContext(configuration.GetConnectionString(Constants.DATABASE)!));

        services.AddScoped<IReadDbContext, AccountsReadDbContext>(_ =>
            new AccountsReadDbContext(configuration.GetConnectionString(Constants.DATABASE)!));

        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Modules.Accounts);

        return services;
    }

    private static IServiceCollection ConfigureCustomOptions(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.JWT));

        services.Configure<AdminOptions>(
            configuration.GetSection(AdminOptions.ADMIN));

        services.Configure<RefreshSessionOptions>(
            configuration.GetSection(RefreshSessionOptions.REFRESH_SESSION));

        return services;
    }

    private static IServiceCollection AddIdentityManagers(this IServiceCollection services)
    {
        services.AddScoped<IPermissionManager, PermissionManager>();

        services.AddScoped<PermissionManager>();

        services.AddScoped<RolePermissionManager>();

        services.AddScoped<IAccountManager, AccountsManager>();

        services.AddScoped<IRefreshSessionManager, RefreshSessionManager>();

        return services;
    }
}
using FileService.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PawsAndHearts.Accounts.Application;

public static class Inject
{
    public static IServiceCollection AddAccountsApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddFileHttpCommunication(configuration);

        return services;
    }
}
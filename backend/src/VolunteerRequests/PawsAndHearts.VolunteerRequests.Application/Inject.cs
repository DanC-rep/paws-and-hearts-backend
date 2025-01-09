using Microsoft.Extensions.DependencyInjection;

namespace PawsAndHearts.VolunteerRequests.Application;

public static class Inject
{
    public static IServiceCollection AddVolunteerRequestsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Inject).Assembly));

        return services;
    }
}
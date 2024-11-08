using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Discussions.Contracts;

namespace PawsAndHearts.Discussions.Presentation;

public static class Inject
{
    public static IServiceCollection AddDiscussionsPresentation(this IServiceCollection services)
    {
        services.AddScoped<IDiscussionsContract, DiscussionsContract>();

        return services;
    } 
}
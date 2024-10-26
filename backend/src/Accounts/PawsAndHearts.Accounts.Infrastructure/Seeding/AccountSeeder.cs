using Microsoft.Extensions.DependencyInjection;

namespace PawsAndHearts.Accounts.Infrastructure.Seeding;

public class AccountSeeder(IServiceScopeFactory serviceScopeFactory)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<AccountsSeederService>();

        await service.SeedAsync(cancellationToken);
    }
}
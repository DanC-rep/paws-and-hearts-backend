using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PawsAndHearts.PetManagement.Infrastructure.Services;

namespace PawsAndHearts.PetManagement.Infrastructure.BackgroundServices;

public class DeleteExpiredEntitiesBackgroundService : BackgroundService
{
    private readonly ILogger<DeleteExpiredEntitiesBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DeleteExpiredEntitiesBackgroundService(
        ILogger<DeleteExpiredEntitiesBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory  = serviceScopeFactory;
    }


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteExpiredEntitiesBackgroundService is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            
            var deleteExpiredVolunteersService = scope.ServiceProvider
                .GetRequiredService<DeleteExpiredVolunteersService>();

            var deleteExpiredPetsService = scope.ServiceProvider
                .GetRequiredService<DeleteExpiredPetsService>();
            
            _logger.LogInformation("DeleteExpiredEntitiesBackgroundService is working.");

            await deleteExpiredVolunteersService.Process(cancellationToken);
            await deleteExpiredPetsService.Process(cancellationToken);

            await Task.Delay(TimeSpan.FromHours(SharedKernel.Constants.FREQUENCY_OF_DELETION),
                cancellationToken);
        }
    }
}
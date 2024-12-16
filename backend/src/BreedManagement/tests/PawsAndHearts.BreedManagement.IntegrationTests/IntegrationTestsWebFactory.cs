using System.Data.Common;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using NSubstitute;
using PawsAndHearts.Accounts.Infrastructure.Seeding;
using PawsAndHearts.BreedManagement.Application.Interfaces;
using PawsAndHearts.BreedManagement.Infrastructure.DbContexts;
using PawsAndHearts.PetManagement.Contracts;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.Web;
using Respawn;
using Testcontainers.PostgreSql;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("paws_and_hearts")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    
    private readonly IPetManagementContract _petManagementContractMock = 
            Substitute.For<IPetManagementContract>();

    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IHostedService));
            services.RemoveAll(typeof(WriteDbContext));
            services.RemoveAll(typeof(ISpeciesReadDbContext));
            services.RemoveAll(typeof(AccountSeeder));

            services.AddScoped<WriteDbContext>(_ =>
                new WriteDbContext(_dbContainer.GetConnectionString()));

            services.AddScoped<ISpeciesReadDbContext, ReadDbContext>(_ =>
                new ReadDbContext(_dbContainer.GetConnectionString()));

            services.RemoveAll(typeof(IPetManagementContract));
            services.AddScoped<IPetManagementContract>(_ => _petManagementContractMock);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        
        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
    
    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["breed-management"]
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public void SetupPetManagementContractMock()
    {
        _petManagementContractMock
            .CheckPetsDoNotHaveSpecies(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(UnitResult.Success<Error>());
    }
}
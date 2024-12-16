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
using PawsAndHearts.Accounts.Contracts;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Accounts.Infrastructure.Seeding;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.Discussions.Infrastructure.DbContexts;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.Web;
using Respawn;
using Testcontainers.PostgreSql;

namespace PawsAndHearts.Discussions.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("paws_and_hearts")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly IAccountsContract _accountsContractMock =
        Substitute.For<IAccountsContract>();

    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IHostedService));
            services.RemoveAll(typeof(WriteDbContext));
            services.RemoveAll(typeof(IDiscussionsReadDbContext));
            services.RemoveAll(typeof(AccountSeeder));

            services.AddScoped<WriteDbContext>(_ =>
                new WriteDbContext(_dbContainer.GetConnectionString()));

            services.AddScoped<IDiscussionsReadDbContext, ReadDbContext>(_ =>
                new ReadDbContext(_dbContainer.GetConnectionString()));

            services.RemoveAll(typeof(IAccountsContract));
            services.AddScoped<IAccountsContract>(_ => _accountsContractMock);
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
            SchemasToInclude = ["discussions"]
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public void SetupAccountsContractMock()
    {
        _accountsContractMock
            .GetUserById(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<UserDto, ErrorList>(new UserDto
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Surname = "Test",
                UserName = "Test"
            }));
    }
}
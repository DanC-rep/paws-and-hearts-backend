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
using PawsAndHearts.Accounts.Infrastructure.Seeding;
using PawsAndHearts.Discussions.Contracts;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;
using PawsAndHearts.Web;
using Respawn;
using Testcontainers.PostgreSql;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("paws_and_hearts")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly IDiscussionsContract _discussionsContractMock =
        Substitute.For<IDiscussionsContract>();

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
            services.RemoveAll(typeof(IVolunteerRequestsReadDbContext));
            services.RemoveAll(typeof(AccountSeeder));

            services.AddScoped<WriteDbContext>(_ =>
                new WriteDbContext(_dbContainer.GetConnectionString()));

            services.AddScoped<IVolunteerRequestsReadDbContext, ReadDbContext>(_ =>
                new ReadDbContext(_dbContainer.GetConnectionString()));

            services.RemoveAll(typeof(IDiscussionsContract));
            services.RemoveAll(typeof(IAccountsContract));

            services.AddScoped<IDiscussionsContract>(_ => _discussionsContractMock);
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
            SchemasToInclude = ["volunteer-requests"]
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public void SetupDiscussionsContractMock()
    {
        var discussionId = Guid.NewGuid();
        
        _discussionsContractMock
            .CreateDiscussion(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<Guid, ErrorList>(discussionId));
    }

    public void SetupAccountsContractMock()
    {
        _accountsContractMock
            .CreateVolunteerAccount(
                Arg.Any<Guid>(),
                Arg.Any<Experience>(),
                Arg.Any<IEnumerable<Requisite>>(),
                Arg.Any<CancellationToken>())
            .Returns(UnitResult.Success<ErrorList>());
    }
}
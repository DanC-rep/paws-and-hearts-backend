using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.BreedManagement.Application.Interfaces;
using PawsAndHearts.BreedManagement.Domain.Entities;
using PawsAndHearts.BreedManagement.Infrastructure.DbContexts;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class BreedManagementTestsBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly Fixture _fixture;
    protected readonly IntegrationTestsWebFactory _factory;
    protected readonly IServiceScope _scope;
    protected readonly WriteDbContext _writeDbContext;
    protected readonly ISpeciesReadDbContext _readDbContext;

    protected BreedManagementTestsBase(IntegrationTestsWebFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _scope = factory.Services.CreateScope();
        _writeDbContext = _scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        _readDbContext = _scope.ServiceProvider.GetRequiredService<ISpeciesReadDbContext>();
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
        
        _scope.Dispose();
    }

    public async Task<Species> SeedSpecies(CancellationToken cancellationToken = default)
    {
        var species = new Species(SpeciesId.NewId(), "test");

        await _writeDbContext.AddAsync(species, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return species;
    }

    public async Task<Species> SeedSpeciesWithBreed(CancellationToken cancellationToken = default)
    {
        var species = await SeedSpecies(cancellationToken);

        var breed = new Breed(BreedId.NewId(), "test", species.Id);

        species.AddBreed(breed);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return species;
    }
}
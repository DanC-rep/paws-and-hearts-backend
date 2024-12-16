using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Domain.Entities;
using PawsAndHearts.PetManagement.Domain.Enums;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.PetManagement.Infrastructure.DbContexts;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.PetManagement.IntegrationTests;

public class PetManagementTestsBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly Fixture _fixture;
    protected readonly IntegrationTestsWebFactory _factory;
    protected readonly IServiceScope _scope;
    protected readonly WriteDbContext _writeDbContext;
    protected readonly IVolunteersReadDbContext _readDbContext;

    protected PetManagementTestsBase(IntegrationTestsWebFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _scope = factory.Services.CreateScope();
        _writeDbContext = _scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        _readDbContext = _scope.ServiceProvider.GetRequiredService<IVolunteersReadDbContext>();
    }

    protected async Task<Guid> SeedVolunteer(CancellationToken cancellationToken = default)
    {
        var volunteer = new Volunteer(
            VolunteerId.NewId(),
            FullName.Create("test", "test", "test").Value,
            Experience.Create(3).Value,
            PhoneNumber.Create("89028689909").Value);

        await _writeDbContext.AddAsync(volunteer, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return volunteer.Id;
    }

    protected async Task<(Guid, Guid)> SeedPet(CancellationToken cancellationToken = default)
    {
        var volunteerId = await SeedVolunteer(cancellationToken);
        
        var pet = new Pet(
            PetId.NewId(),
            "test",
            "test",
            new PetIdentity(SpeciesId.NewId(), BreedId.NewId()),
            Color.Create("test").Value,
            "test",
            Address.Create("test", "test", "test", "test").Value,
            new PetMetrics(10, 10),
            PhoneNumber.Create("89202908765").Value,
            true,
            BirthDate.Create(DateTime.UtcNow).Value,
            true,
            HelpStatus.NeedForHelp,
            CreationDate.Create(DateTime.UtcNow).Value,
            new List<Requisite>());

        var volunteer = await _writeDbContext.Volunteers.FirstAsync(cancellationToken);

        volunteer.AddPet(pet);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return (volunteerId, pet.Id);
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
}
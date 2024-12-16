using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.UseCases.CreatePet;

namespace PawsAndHearts.PetManagement.IntegrationTests.Pets;

public class CreatePetTest : PetManagementTestsBase
{
    private readonly ICommandHandler<Guid, CreatePetCommand> _sut;

    public CreatePetTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreatePetCommand>>();
    }

    [Fact]
    public async Task Add_Pet_To_Database()
    {
        // arrange
        _factory.SetupBreedManagementContractMock();
        
        var cancellationToken = new CancellationTokenSource().Token;

        var volunteerId = await SeedVolunteer(cancellationToken);

        var command = _fixture.CreateAddPetCommand(volunteerId);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var pet = _readDbContext.Pets
            .FirstOrDefaultAsync(p => p.Id == result.Value, cancellationToken);

        pet.Should().NotBeNull();
    }
}
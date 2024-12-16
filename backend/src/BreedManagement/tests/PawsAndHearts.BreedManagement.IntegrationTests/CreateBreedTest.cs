using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.BreedManagement.Application.UseCases.CreateBreed;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class CreateBreedTest : BreedManagementTestsBase
{
    private readonly ICommandHandler<Guid, CreateBreedCommand> _sut;

    public CreateBreedTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateBreedCommand>>();
    }

    [Fact]
    public async Task Add_Breed_To_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var species = await SeedSpecies(cancellationToken);

        var command = _fixture.CreateAddBreedCommand(species.Id);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var breed = await _readDbContext.Breeds
            .FirstOrDefaultAsync(b => b.Id == result.Value, cancellationToken);

        breed.Should().NotBeNull();
    }
}
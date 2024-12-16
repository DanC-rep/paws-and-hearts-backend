using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.BreedManagement.Application.UseCases.DeleteBreed;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class DeleteBreedTest : BreedManagementTestsBase
{
    private readonly ICommandHandler<Guid, DeleteBreedCommand> _sut;

    public DeleteBreedTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeleteBreedCommand>>();
    }

    [Fact]
    public async Task Remove_Breed_From_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var species = await SeedSpeciesWithBreed(cancellationToken);
        var breed = species.Breeds[0];

        var command = new DeleteBreedCommand(species.Id, breed.Id);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var breeds = await _readDbContext.Breeds.ToListAsync(cancellationToken);

        breeds.Should().BeEmpty();
    }
}
using AutoFixture;
using PawsAndHearts.BreedManagement.Application.UseCases.CreateBreed;
using PawsAndHearts.BreedManagement.Application.UseCases.CreateSpecies;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public static class FixtureExtensions
{
    public static CreateSpeciesCommand CreateAddSpeciesCommand(this IFixture fixture)
    {
        return fixture.Create<CreateSpeciesCommand>();
    }

    public static CreateBreedCommand CreateAddBreedCommand(
        this IFixture fixture,
        Guid speciesId)
    {
        return fixture.Build<CreateBreedCommand>()
            .With(c => c.SpeciesId, speciesId)
            .Create();
    }
}
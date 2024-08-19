using CSharpFunctionalExtensions;
using PawsAndHearts.Domain.Shared;

namespace PawsAndHearts.Domain.ValueObjects;

public record PetIdentity
{
    private PetIdentity(SpeciesId speciesId, BreedId breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }
    
    public SpeciesId SpeciesId { get; } = default!;

    public BreedId BreedId { get; } = default!;

    public Result<PetIdentity, Error> Create(SpeciesId speciesId, BreedId breedId)
    {
        return new PetIdentity(speciesId, breedId);
    }
}
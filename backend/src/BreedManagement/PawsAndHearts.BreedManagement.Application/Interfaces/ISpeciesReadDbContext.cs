using PawsAndHearts.BreedManagement.Contracts.Dtos;

namespace PawsAndHearts.BreedManagement.Application.Interfaces;

public interface ISpeciesReadDbContext
{
    IQueryable<SpeciesDto> Species { get; }
    
    IQueryable<BreedDto> Breeds { get; }
}
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.PetManagement.Contracts.Dtos;

namespace PawsAndHearts.PetManagement.Contracts.Responses;

public class PetResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;

    public string Description { get; init; } = default!;
    
    public Guid SpeciesId { get; init; }
    
    public Guid BreedId { get; init; }
    
    public Guid VolunteerId { get; init; }
    
    public string Color { get; init; } = default!;

    public string HealthInfo { get; init; } = default!;

    public string City { get; init; } = default!;

    public string Street { get; init; } = default!;

    public string House { get; init; } = default!;
    
    public string? Flat { get; init; }
    
    public double Height { get; init; }
    
    public double Weight { get; init; }

    public string PhoneNumber { get; init; } = default!;
    
    public bool IsNeutered { get; init; }
    
    public DateTime BirthDate { get; init; }
    
    public bool IsVaccinated { get; init; }

    public string HelpStatus { get; init; } = default!;
    
    public DateTime CreationDate { get; init; }
    
    public int Position { get; init; }

    public IEnumerable<RequisiteDto> Requisites { get; init; } = [];
    
    public IEnumerable<PetPhotoResponse>? PetPhotos { get; init; } = [];
    
    public bool IsDeleted { get; init; }
    
    public static PetResponse Create(PetDto petDto, IEnumerable<PetPhotoResponse>? photos = null) =>
        new PetResponse
        {
            Id = petDto.Id,
            Name = petDto.Name,
            Description = petDto.Description,
            SpeciesId = petDto.SpeciesId,
            BreedId = petDto.BreedId,
            VolunteerId = petDto.VolunteerId,
            Color = petDto.Color,
            HealthInfo = petDto.HealthInfo,
            City = petDto.City,
            Street = petDto.Street,
            House = petDto.House,
            Flat = petDto.Flat,
            Height = petDto.Height,
            Weight = petDto.Weight,
            PhoneNumber = petDto.PhoneNumber,
            IsNeutered = petDto.IsNeutered,
            BirthDate = petDto.BirthDate,
            IsVaccinated = petDto.IsVaccinated,
            HelpStatus = petDto.HelpStatus,
            CreationDate = petDto.CreationDate,
            Position = petDto.Position,
            Requisites = petDto.Requisites,
            PetPhotos = photos,
            IsDeleted = petDto.IsDeleted
        };
}

public record PetPhotoResponse(Guid Id, string DownloadPath, bool IsMain);
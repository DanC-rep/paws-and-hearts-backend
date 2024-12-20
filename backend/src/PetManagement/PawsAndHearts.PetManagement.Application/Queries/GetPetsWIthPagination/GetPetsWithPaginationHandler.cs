using System.Linq.Expressions;
using FileService.Communication;
using FileService.Contracts.Requests;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.Core.Models;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Contracts.Dtos;
using PawsAndHearts.PetManagement.Contracts.Responses;

namespace PawsAndHearts.PetManagement.Application.Queries.GetPetsWIthPagination;

public class GetPetsWithPaginationHandler : IQueryHandler<PagedList<PetResponse>, GetPetsWithPaginationQuery>
{
    private readonly IVolunteersReadDbContext _volunteersReadDbContext;
    private readonly IFileService _fileService;

    public GetPetsWithPaginationHandler(
        IVolunteersReadDbContext volunteersReadDbContext,
        IFileService fileService)
    {
        _volunteersReadDbContext = volunteersReadDbContext;
        _fileService = fileService;
    }

    public async Task<PagedList<PetResponse>> Handle(
        GetPetsWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var petsQuery = ApplyFilters(_volunteersReadDbContext.Pets, query);

        var keySelector = SortByProperty(query.SortBy);

        petsQuery = query.SortDirection?.ToLower() == "desc"
            ? petsQuery.OrderByDescending(keySelector)
            : petsQuery.OrderBy(keySelector);

        var petsResponse = new List<PetResponse>();

        foreach (var petDto in petsQuery)
        {
            if (petDto.PetPhotos is null)
                 petsResponse.Add(PetResponse.Create(petDto));

            var petPhotosIds = petDto.PetPhotos?.Select(p => p.FileId);

            var filesRequest = new GetFilesByIdsRequest(petPhotosIds!);

            var filesResponse = await _fileService.GetFilesByIds(filesRequest, cancellationToken);

            var urls = filesResponse.Value.FilesInfo.ToDictionary(f => f.Id, f => f.DownloadPath);

            var petPhotosResponse = petDto.PetPhotos!
                .Where(p => urls.ContainsKey(p.FileId))
                .Select(p => new PetPhotoResponse(p.FileId, urls[p.FileId], p.IsMain));
            
            petsResponse.Add(PetResponse.Create(petDto, petPhotosResponse));
        }
        
        var result = petsResponse.ToPagedList(query.Page, query.PageSize);

        return result;
    }
    
    private static Expression<Func<PetDto, object>> SortByProperty(string? sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
            return volunteer => volunteer.Id;

        Expression<Func<PetDto, object>> keySelector = sortBy.ToLower() switch
        {
            "name" => pet => pet.Name,
            "birthDate" => pet => pet.BirthDate,
            "species" => pet => pet.SpeciesId,
            "breed" => pet => pet.BreedId,
            "color" => pet => pet.Color,
            "city" => pet => pet.City,
            "street" => pet => pet.Street,
            "house" => pet => pet.House,
            "flat" => pet => pet.Flat ?? string.Empty,
            "height" => pet => pet.Height,
            "weight" => pet => pet.Weight,
            "volunteer" => pet => pet.VolunteerId,
            _ => pet => pet.Id
        };
        
        return keySelector;
    }

    private static IQueryable<PetDto> ApplyFilters(IQueryable<PetDto> petsQuery, GetPetsWithPaginationQuery query)
    {
        return petsQuery
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), pet => pet.Name.Contains(query.Name!))
            .WhereIf(query.VolunteerId.HasValue, pet => pet.VolunteerId == query.VolunteerId)
            .WhereIf(query.BreedId.HasValue, pet => pet.BreedId == query.BreedId)
            .WhereIf(query.SpeciesId.HasValue, pet => pet.SpeciesId == query.SpeciesId)
            .WhereIf(!string.IsNullOrWhiteSpace(query.Color), pet => pet.Color == query.Color)
            .WhereIf(!string.IsNullOrWhiteSpace(query.City), pet => pet.City == query.City)
            .WhereIf(!string.IsNullOrWhiteSpace(query.Street), pet => pet.Street == query.Street)
            .WhereIf(query.MinAge.HasValue, pet => (DateTime.Now - pet.BirthDate).TotalDays / 365 >= query.MinAge)
            .WhereIf(query.MaxAge.HasValue, pet => (DateTime.Now - pet.BirthDate).TotalDays / 365 <= query.MaxAge)
            .WhereIf(query.MinHeight.HasValue, pet => pet.Height >= query.MinHeight)
            .WhereIf(query.MaxHeight.HasValue, pet => pet.Height <= query.MaxHeight)
            .WhereIf(query.MinWeight.HasValue, pet => pet.Weight >= query.MinWeight)
            .WhereIf(query.MaxWeight.HasValue, pet => pet.Weight <= query.MaxWeight)
            .WhereIf(!string.IsNullOrWhiteSpace(query.HelpStatus), pet => pet.HelpStatus == query.HelpStatus)
            .WhereIf(query.IsNeutered.HasValue, pet => pet.IsNeutered == query.IsNeutered)
            .WhereIf(query.IsVaccinated.HasValue, pet => pet.IsVaccinated == query.IsVaccinated);
    }
}
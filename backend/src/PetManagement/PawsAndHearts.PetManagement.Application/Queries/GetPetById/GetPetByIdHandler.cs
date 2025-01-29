using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Caching;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Contracts.Dtos;
using PawsAndHearts.PetManagement.Contracts.Responses;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.PetManagement.Application.Queries.GetPetById;

public class GetPetByIdHandler : IQueryHandlerWithResult<PetResponse, GetPetByIdQuery>
{
    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(5)
    };
    
    private readonly IVolunteersReadDbContext _context;
    private readonly IFileService _fileService;
    private readonly ICacheService _cacheService;

    public GetPetByIdHandler(
        IVolunteersReadDbContext context,
        IFileService fileService,
        ICacheService cacheService)
    {
        _context = context;
        _fileService = fileService;
        _cacheService = cacheService;
    }

    public async Task<Result<PetResponse, ErrorList>> Handle(
        GetPetByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var key = "pets_" + query.Id;

        var petDto = await _cacheService.GetOrCreateAsync(
            key,
            _cacheOptions,
            async () => await _context.Pets.FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken),
            cancellationToken);

        if (petDto is null)
            return Errors.General.NotFound(query.Id).ToErrorList();

        if (petDto.PetPhotos is null)
            return PetResponse.Create(petDto);

        var petPhotosIds = petDto.PetPhotos?.Select(p => p.FileId);

        var filesRequest = new GetFilesByIdsRequest(petPhotosIds!);

        var filesResponse = await _fileService.GetFilesByIds(filesRequest, cancellationToken);

        var urls = filesResponse.Value.FilesInfo.ToDictionary(f => f.Id, f => f.DownloadPath);

        var petPhotosResponse = petDto.PetPhotos!
            .Where(p => urls.ContainsKey(p.FileId))
            .Select(p => new PetPhotoResponse(p.FileId, urls[p.FileId], p.IsMain));
        
        return PetResponse.Create(petDto, petPhotosResponse);
    }
}
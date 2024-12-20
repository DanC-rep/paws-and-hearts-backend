using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Contracts.Dtos;
using PawsAndHearts.PetManagement.Contracts.Responses;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.PetManagement.Application.Queries.GetPetById;

public class GetPetByIdHandler : IQueryHandlerWithResult<PetResponse, GetPetByIdQuery>
{
    private readonly IVolunteersReadDbContext _context;
    private readonly IFileService _fileService;

    public GetPetByIdHandler(
        IVolunteersReadDbContext context,
        IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<PetResponse, ErrorList>> Handle(
        GetPetByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var petDto = await _context.Pets.FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

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
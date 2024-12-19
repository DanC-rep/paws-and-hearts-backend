using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.PetManagement.Application.UseCases.DeletePetForce;

public class DeletePetForceHandler : ICommandHandler<Guid, DeletePetForceCommand>
{
    private const string BUCKET_NAME = "photos";
    
    private readonly IVolunteersRepository _volunteersVolunteersRepository;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePetForceHandler> _logger;

    public DeletePetForceHandler(
        IVolunteersRepository volunteersRepository,
        IFileService fileService,
        [FromKeyedServices(Modules.PetManagement)] IUnitOfWork unitOfWork, 
        ILogger<DeletePetForceHandler> logger)
    {
        _volunteersVolunteersRepository = volunteersRepository;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        DeletePetForceCommand command,
        CancellationToken cancellationToken = default)
    {
        var volunteerResult = await _volunteersVolunteersRepository.GetById(command.VolunteerId, cancellationToken);

        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petResult = volunteerResult.Value.GetPetById(command.PetId);

        if (petResult.IsFailure)
            return petResult.Error.ToErrorList();
            
        var petPhotosIds = (petResult.Value.PetPhotos ?? new List<PetPhoto>())
            .Select(p => p.FileId).ToList();

        var deletePhotosRequest = new DeleteFilesRequest(petPhotosIds);
        
        var deletePhotosResponse = await _fileService.DeleteFiles(deletePhotosRequest, cancellationToken);
        
        if (deletePhotosResponse.IsFailure)
            return Error.Failure("delete.files", deletePhotosResponse.Error).ToErrorList();
            
        volunteerResult.Value.DeletePet(petResult.Value);

        await _unitOfWork.SaveChanges(cancellationToken);
            
        _logger.LogInformation("Pet was force deleted with id {petId}", command.PetId);

        return command.PetId;
    }
}
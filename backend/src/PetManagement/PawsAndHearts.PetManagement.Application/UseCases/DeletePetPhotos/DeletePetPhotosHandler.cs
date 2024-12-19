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

namespace PawsAndHearts.PetManagement.Application.UseCases.DeletePetPhotos;

public class DeletePetPhotosHandler : ICommandHandler<DeletePetPhotosCommand>
{
    private const string BUCKET_NAME = "photos";

    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePetPhotosHandler> _logger;

    public DeletePetPhotosHandler(
        IVolunteersRepository volunteersRepository,
        IFileService fileService,
        ILogger<DeletePetPhotosHandler> logger,
        [FromKeyedServices(Modules.PetManagement)] IUnitOfWork unitOfWork)
    {
        _volunteersRepository = volunteersRepository;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        DeletePetPhotosCommand command,
        CancellationToken cancellationToken = default)
    {
        var volunteerResult = await _volunteersRepository.GetById(command.VolunteerId, cancellationToken);

        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petResult = volunteerResult.Value.GetPetById(command.PetId);

        if (petResult.IsFailure)
            return petResult.Error.ToErrorList();
        
        var petPhotosIds = (petResult.Value.PetPhotos ?? new List<PetPhoto>())
            .Select(p => p.FileId).ToList();

        var request = new DeleteFilesRequest(petPhotosIds);

        var deleteResult = await _fileService.DeleteFiles(request, cancellationToken);

        if (deleteResult.IsFailure)
            return Error.Failure("delete.files", deleteResult.Error).ToErrorList();
        
        volunteerResult.Value.DeletePetPhotos(petResult.Value);
            
        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Photos was delete for pet with id {petId}", command.PetId);
            
        return Result.Success<ErrorList>();
    }
}
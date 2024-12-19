using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.PetManagement.Application.UseCases.CompleteUploadPhotosToPet;

public class CompleteUploadPhotosToPetHandler : 
    ICommandHandler<IEnumerable<CompleteMultipartUploadResponse>, CompleteUploadPhotosToPetCommand>
{
    private readonly IVolunteersRepository _repository;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteUploadPhotosToPetHandler> _logger;

    public CompleteUploadPhotosToPetHandler(
        IVolunteersRepository repository,
        IFileService fileService,
        ILogger<CompleteUploadPhotosToPetHandler> logger,
        [FromKeyedServices(Modules.PetManagement)] IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<IEnumerable<CompleteMultipartUploadResponse>, ErrorList>> Handle(
        CompleteUploadPhotosToPetCommand command, 
        CancellationToken cancellationToken = default)
    {
        var volunteerResult = await _repository.GetById(command.VolunteerId, cancellationToken);

        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petResult = volunteerResult.Value.GetPetById(command.PetId);

        if (petResult.IsFailure)
            return petResult.Error.ToErrorList();
        
        var responses = new List<CompleteMultipartUploadResponse>();
        var photos = new List<PetPhoto>();

        foreach (var file in command.Files)
        {
            var uploadRequest = new CompleteMultipartRequest(file.UploadId, file.Parts);
            
            var result = await _fileService.CompleteMultipartUpload(
                uploadRequest, file.Key, cancellationToken);
            
            if (result.IsFailure)
                return Error.Failure("complete.multipart.upload", result.Error).ToErrorList();
            
            var photo = PetPhoto.Create(result.Value.FileId, false);

            if (photo.IsFailure)
                return photo.Error.ToErrorList();

            photos.Add(photo.Value);
            responses.Add(result.Value);
        }

        volunteerResult.Value.AddPetPhotos(petResult.Value, photos);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Photos was uploaded for pet with id {petID}", command.PetId);

        return responses;
    }
}
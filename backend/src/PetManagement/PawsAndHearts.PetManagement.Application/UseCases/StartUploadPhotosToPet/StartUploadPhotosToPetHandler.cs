using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Responses;
using FluentValidation;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.PetManagement.Application.UseCases.StartUploadPhotosToPet;

public class StartUploadPhotosToPetHandler : 
    ICommandHandler<IEnumerable<StartMultipartUploadResponse>, StartUploadPhotosToPetCommand>
{
    private readonly IVolunteersRepository _repository;
    private readonly IFileService  _fileService;
    private readonly IValidator<StartUploadPhotosToPetCommand> _validator;

    public StartUploadPhotosToPetHandler(
        IVolunteersRepository repository,
        IFileService fileService,
        IValidator<StartUploadPhotosToPetCommand> validator)
    {
        _fileService = fileService;
        _validator = validator;
        _repository = repository;
    }

    public async Task<Result<IEnumerable<StartMultipartUploadResponse>, ErrorList>> Handle(
        StartUploadPhotosToPetCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var volunteerResult = await _repository.GetById(command.VolunteerId, cancellationToken);

        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petResult = volunteerResult.Value.GetPetById(command.PetId);

        if (petResult.IsFailure)
            return petResult.Error.ToErrorList();

        var responses = new List<StartMultipartUploadResponse>();

        foreach (var file in command.Files)
        {
            var validationPhotoResult = Photo.Validate(file.FileName, file.ContentType);

            if (validationPhotoResult.IsFailure)
                return validationPhotoResult.Error.ToErrorList();

            var result = await _fileService.StartMultipartUpload(file, cancellationToken);

            if (result.IsFailure)
                return Error.Failure("start.upload.file", result.Error).ToErrorList();
            
            responses.Add(result.Value);
        }

        return responses;
    }
}
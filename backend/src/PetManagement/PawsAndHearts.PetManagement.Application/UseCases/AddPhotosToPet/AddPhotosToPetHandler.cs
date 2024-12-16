/*using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.Core.Messaging;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.FileProvider;
using PawsAndHearts.SharedKernel.Interfaces;
using PawsAndHearts.SharedKernel.ValueObjects;
using FileInfo = PawsAndHearts.SharedKernel.FileProvider.FileInfo;

namespace PawsAndHearts.PetManagement.Application.UseCases.AddPhotosToPet;

public class AddPhotosToPetHandler : ICommandHandler<FilePathList, AddPhotosToPetCommand>
{
    private const string BUCKET_NAME = "photos";
    
    private readonly IVolunteersRepository _repository;
    private readonly IFileProvider _fileProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueue<IEnumerable<FileInfo>> _messageQueue;
    private readonly IValidator<AddPhotosToPetCommand> _validator;
    private readonly ILogger<AddPhotosToPetHandler> _logger;

    public AddPhotosToPetHandler(
        IVolunteersRepository repository, 
        ILogger<AddPhotosToPetHandler> logger, 
        IFileProvider fileProvider,
        IValidator<AddPhotosToPetCommand> validator,
        [FromKeyedServices(Modules.PetManagement)] IUnitOfWork unitOfWork, 
        IMessageQueue<IEnumerable<FileInfo>> messageQueue)
    {
        _repository = repository;
        _logger = logger;
        _fileProvider = fileProvider;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _messageQueue = messageQueue;
    }

    public async Task<Result<FilePathList, ErrorList>> Handle(
        AddPhotosToPetCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);
        
        try
        {
            var volunteerResult = await _repository.GetById(command.VolunteerId, cancellationToken);

            if (volunteerResult.IsFailure)
                return volunteerResult.Error.ToErrorList();

            var petResult = volunteerResult.Value.GetPetById(command.PetId);

            if (petResult.IsFailure)
                return petResult.Error.ToErrorList();

            List<UploadFileData> fileDatas = [];

            foreach (var file in command.Files)
            {
                var extension = Path.GetExtension(file.FileName);

                var filePathResult = FilePath.Create(Guid.NewGuid(), extension);

                if (filePathResult.IsFailure)
                    return filePathResult.Error.ToErrorList();

                var fileData = new UploadFileData(file.Content, new FileInfo(filePathResult.Value, BUCKET_NAME));

                fileDatas.Add(fileData);
            }

            var petPhotos = fileDatas
                .Select(f => f.Info.FilePath)
                .Select(f => PetPhoto.Create(f, false).Value)
                .ToList();
            
            volunteerResult.Value.AddPetPhotos(petResult.Value, petPhotos);

            await _unitOfWork.SaveChanges(cancellationToken);

            var uploadResult = await _fileProvider.UploadFiles(fileDatas, cancellationToken);

            if (uploadResult.IsFailure)
            {
                await _messageQueue.WriteAsync(fileDatas.Select(f => f.Info), cancellationToken);
                return uploadResult.Error.ToErrorList();
            }

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Files was uploaded for pet {petId}", command.PetId);

            return uploadResult.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to upload files for pet {petId}", command.PetId);
            
            await transaction.RollbackAsync(cancellationToken);

            return Error.Failure("pet.upload.files", "Can not upload files for pet")
                .ToErrorList();
        }
    }
}*/
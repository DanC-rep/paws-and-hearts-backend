using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Messaging;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.Interfaces;
using FileInfo = PawsAndHearts.SharedKernel.FileProvider.FileInfo;

namespace PawsAndHearts.PetManagement.Application.UseCases.DeletePetPhotos;

public class DeletePetPhotosHandler : ICommandHandler<DeletePetPhotosCommand>
{
    private const string BUCKET_NAME = "photos";

    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IFileProvider _fileProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueue<IEnumerable<FileInfo>> _messageQueue;
    private readonly ILogger<DeletePetPhotosHandler> _logger;

    public DeletePetPhotosHandler(
        IVolunteersRepository volunteersRepository,
        IFileProvider fileProvider,
        [FromKeyedServices(Modules.PetManagement)] IUnitOfWork unitOfWork, 
        ILogger<DeletePetPhotosHandler> logger, 
        IMessageQueue<IEnumerable<FileInfo>> messageQueue)
    {
        _volunteersRepository = volunteersRepository;
        _fileProvider = fileProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _messageQueue = messageQueue;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        DeletePetPhotosCommand command,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);
        
        try
        {
            var volunteerResult = await _volunteersRepository.GetById(command.VolunteerId, cancellationToken);

            if (volunteerResult.IsFailure)
                return volunteerResult.Error.ToErrorList();

            var petResult = volunteerResult.Value.GetPetById(command.PetId);

            if (petResult.IsFailure)
                return petResult.Error.ToErrorList();
        
            var petPreviousPhotosInfo = petResult.Value.PetPhotos!
                .Select(p => new FileInfo(p.Path, BUCKET_NAME)).ToList();
            
            volunteerResult.Value.DeletePetPhotos(petResult.Value);
            
            await _unitOfWork.SaveChanges(cancellationToken);

            var deleteResult = await _fileProvider.DeleteFiles(petPreviousPhotosInfo, cancellationToken);

            if (deleteResult.IsFailure)
            {
                await _messageQueue.WriteAsync(petPreviousPhotosInfo, cancellationToken);
                return deleteResult.Error.ToErrorList();
            }
            
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Files was deleted for pet {petId}", command.PetId);

            return Result.Success<ErrorList>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to delete files for pet {petId}", command.PetId);
            
            await transaction.RollbackAsync(cancellationToken);

            return Error.Failure("pet.delete.files", "Can not delete files for pet")
                .ToErrorList();
        }
    }
}
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Messaging;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.Interfaces;
using FileInfo = PawsAndHearts.SharedKernel.FileProvider.FileInfo;

namespace PawsAndHearts.PetManagement.Application.UseCases.DeletePetForce;

public class DeletePetForceHandler : ICommandHandler<Guid, DeletePetForceCommand>
{
    private const string BUCKET_NAME = "photos";
    
    private readonly IVolunteersRepository _volunteersVolunteersRepository;
    private readonly IFileProvider _fileProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueue<IEnumerable<FileInfo>> _messageQueue;
    private readonly ILogger<DeletePetForceHandler> _logger;

    public DeletePetForceHandler(
        IVolunteersRepository volunteersRepository,
        IFileProvider fileProvider,
        [FromKeyedServices(Modules.PetManagement)] IUnitOfWork unitOfWork, 
        IMessageQueue<IEnumerable<FileInfo>> messageQueue,
        ILogger<DeletePetForceHandler> logger)
    {
        _volunteersVolunteersRepository = volunteersRepository;
        _fileProvider = fileProvider;
        _unitOfWork = unitOfWork;
        _messageQueue = messageQueue;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        DeletePetForceCommand command,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var volunteerResult = await _volunteersVolunteersRepository.GetById(command.VolunteerId, cancellationToken);

            if (volunteerResult.IsFailure)
                return volunteerResult.Error.ToErrorList();

            var petResult = volunteerResult.Value.GetPetById(command.PetId);

            if (petResult.IsFailure)
                return petResult.Error.ToErrorList();
            
            var petPreviousPhotosInfo = (petResult.Value.PetPhotos ?? new List<PetPhoto>())
                .Select(p => new FileInfo(p.Path, BUCKET_NAME)).ToList();
            
            volunteerResult.Value.DeletePet(petResult.Value);

            await _unitOfWork.SaveChanges(cancellationToken);
            
            var deleteResult = await _fileProvider.DeleteFiles(petPreviousPhotosInfo, cancellationToken);

            if (deleteResult.IsFailure)
            {
                await _messageQueue.WriteAsync(petPreviousPhotosInfo, cancellationToken);
                return deleteResult.Error.ToErrorList();
            }
            
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Pet was force deleted with id {petId}", command.PetId);

            return command.PetId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to delete pet with id {petId}", command.PetId);
            
            await transaction.RollbackAsync(cancellationToken);

            return Error.Failure("pet.delete", "Can not delete pet")
                .ToErrorList();
        }
    }
}
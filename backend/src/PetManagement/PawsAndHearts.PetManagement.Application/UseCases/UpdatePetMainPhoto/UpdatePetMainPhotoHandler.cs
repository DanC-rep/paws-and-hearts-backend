using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.PetManagement.Application.UseCases.UpdatePetMainPhoto;

public class UpdatePetMainPhotoHandler : ICommandHandler<Guid, UpdatePetMainPhotoCommand>
{
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePetMainPhotoHandler> _logger;

    public UpdatePetMainPhotoHandler(
        IVolunteersRepository repository,
        [FromKeyedServices(Modules.PetManagement)] IUnitOfWork unitOfWork, 
        ILogger<UpdatePetMainPhotoHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdatePetMainPhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        var volunteerResult = await _repository.GetById(command.VolunteerId, cancellationToken);

        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petResult = volunteerResult.Value.GetPetById(command.PetId);

        if (petResult.IsFailure)
            return petResult.Error.ToErrorList();

        var petPhoto = PetPhoto.Create(command.FileId, true);
        
        var updateResult = volunteerResult.Value.UpdatePetMainPhoto(petResult.Value, petPhoto.Value);

        if (updateResult.IsFailure)
            return updateResult.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Pet main photo was updated with pet id: {petId}", command.PetId);

        return updateResult.Value;
    }
}
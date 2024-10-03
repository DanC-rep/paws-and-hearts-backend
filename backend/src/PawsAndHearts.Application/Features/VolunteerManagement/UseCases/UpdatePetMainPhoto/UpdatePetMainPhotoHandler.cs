﻿using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Application.Interfaces;
using PawsAndHearts.Domain.Shared;
using PawsAndHearts.Domain.Shared.ValueObjects;
using PawsAndHearts.Domain.Volunteer.ValueObjects;

namespace PawsAndHearts.Application.Features.VolunteerManagement.UseCases.UpdatePetMainPhoto;

public class UpdatePetMainPhotoHandler : ICommandHandler<FilePath, UpdatePetMainPhotoCommand>
{
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePetMainPhotoHandler> _logger;

    public UpdatePetMainPhotoHandler(
        IVolunteersRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePetMainPhotoHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<FilePath, ErrorList>> Handle(
        UpdatePetMainPhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        var volunteerResult = await _repository.GetById(command.VolunteerId, cancellationToken);

        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petResult = volunteerResult.Value.GetPetById(command.PetId);

        if (petResult.IsFailure)
            return petResult.Error.ToErrorList();

        var filePath = FilePath.Create(command.FilePath);

        if (filePath.IsFailure)
            return filePath.Error.ToErrorList();

        var petPhoto = PetPhoto.Create(filePath.Value, true);
        
        var updateResult = volunteerResult.Value.UpdatePetMainPhoto(petResult.Value, petPhoto.Value);

        if (updateResult.IsFailure)
            return updateResult.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Pet main photo was updated with pet id: {petId}", command.PetId);

        return updateResult.Value;
    }
}
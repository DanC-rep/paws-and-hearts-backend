using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.UpdateVolunteerRequest;

public class UpdateVolunteerRequestHandler : ICommandHandler<UpdateVolunteerRequestCommand>
{
    private readonly IVolunteerRequestsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateVolunteerRequestCommand> _validator;
    private readonly ILogger<UpdateVolunteerRequestHandler> _logger;

    public UpdateVolunteerRequestHandler(
        IVolunteerRequestsRepository repository,
        [FromKeyedServices(Modules.VolunteerRequests)]IUnitOfWork unitOfWork,
        IValidator<UpdateVolunteerRequestCommand> validator,
        ILogger<UpdateVolunteerRequestHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        UpdateVolunteerRequestCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var volunteerRequestResult = await _repository
            .GetById(command.VolunteerRequestId, cancellationToken);

        if (volunteerRequestResult.IsFailure)
            return volunteerRequestResult.Error.ToErrorList();

        if (volunteerRequestResult.Value.UserId != command.UserId)
            return Error.Failure("access.denied", "Request belong another user").ToErrorList();

        var experience = Experience.Create(command.Experience).Value;
        
        var requisites = command.Requisites.Select(r =>
            Requisite.Create(r.Name, r.Description).Value).ToList();

        var volunteerInfo = new VolunteerInfo(experience, requisites);
        
        var result = volunteerRequestResult.Value.Update(volunteerInfo);

        if (result.IsFailure)
            return result.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Volunteer request {requestId} was updated", command.VolunteerRequestId);

        return Result.Success<ErrorList>();
    }
}
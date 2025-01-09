using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Domain.Entities;
using PawsAndHearts.VolunteerRequests.Domain.Events;
using PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.CreateVolunteerRequest;

public class CreateVolunteerRequestHandler : ICommandHandler<Guid, CreateVolunteerRequestCommand>
{
    private readonly IVolunteerRequestsRepository _volunteerRequestsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateVolunteerRequestHandler> _logger;
    private readonly IValidator<CreateVolunteerRequestCommand> _validator;
    private readonly IPublisher _publisher;

    public CreateVolunteerRequestHandler(
        IVolunteerRequestsRepository volunteerRequestsRepository,
        [FromKeyedServices(Modules.VolunteerRequests)]  IUnitOfWork unitOfWork,
        ILogger<CreateVolunteerRequestHandler> logger,
        IValidator<CreateVolunteerRequestCommand> validator,
        IPublisher publisher)
    {
        _volunteerRequestsRepository = volunteerRequestsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _validator = validator;
        _publisher = publisher;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateVolunteerRequestCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            await _publisher.PublishDomainEvent(
                new UserRestrictionOnVolunteerRequestCheckedEvent(command.UserId), cancellationToken);
        
            var requisites = command.Requisites.Select(r =>
                Requisite.Create(r.Name, r.Description).Value).ToList();
        
            var experience = Experience.Create(command.Experience).Value;

            var volunteerInfo = new VolunteerInfo(experience, requisites);
        
            var volunteerRequestId = VolunteerRequestId.NewId();

            var volunteerRequest = VolunteerRequest.CreateRequest(
                volunteerRequestId, 
                command.UserId, 
                volunteerInfo);
        
            var result = await _volunteerRequestsRepository.Add(volunteerRequest, cancellationToken);

            await _unitOfWork.SaveChanges(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        
            _logger.LogInformation("User {userId} created volunteer request with id {volunteerRequestId}",
                command.UserId, volunteerRequestId.Value);

            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            
            _logger.LogError("Can not create volunteer request handler");
            
            return Error.Failure("volunteer.request.create", 
                "Can not create volunteer request handler").ToErrorList();
        }
    }
}
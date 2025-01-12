using CSharpFunctionalExtensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Contracts.Messaging;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.ApproveVolunteerRequest;

public class ApproveVolunteerRequestHandler : ICommandHandler<ApproveVolunteerRequestCommand>
{
    private readonly IVolunteerRequestsRepository _repository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveVolunteerRequestHandler> _logger;

    public ApproveVolunteerRequestHandler(
        IVolunteerRequestsRepository repository,
        IOutboxRepository outboxRepository,
        [FromKeyedServices(Modules.VolunteerRequests)] IUnitOfWork unitOfWork,
        ILogger<ApproveVolunteerRequestHandler> logger)
    {
        _repository = repository;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        ApproveVolunteerRequestCommand command, 
        CancellationToken cancellationToken = default)
    {
        var volunteerRequestResult = await _repository
            .GetById(command.VolunteerRequestId, cancellationToken);

        if (volunteerRequestResult.IsFailure)
            return volunteerRequestResult.Error.ToErrorList();

        var result = volunteerRequestResult.Value.Approve();

        if (result.IsFailure)
            return result.Error.ToErrorList();
        
        var requisitesDtos = volunteerRequestResult.Value.VolunteerInfo.Requisites
            .Select(r => new RequisiteDto(r.Name, r.Description));

        var @event = new ApproveVolunteerRequestEvent(
            volunteerRequestResult.Value.UserId,
            volunteerRequestResult.Value.VolunteerInfo.Experience.Value,
            requisitesDtos);

        await _outboxRepository.Add(@event, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);
            
        _logger.LogInformation("Volunteer request {requestId} was approved successfully", 
            command.VolunteerRequestId);

        return Result.Success<ErrorList>();
    }
}
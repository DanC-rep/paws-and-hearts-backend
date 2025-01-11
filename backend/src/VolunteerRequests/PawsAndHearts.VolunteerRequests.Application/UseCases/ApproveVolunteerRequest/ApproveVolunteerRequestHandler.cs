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
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveVolunteerRequestHandler> _logger;

    public ApproveVolunteerRequestHandler(
        IVolunteerRequestsRepository repository,
        IPublishEndpoint publishEndpoint,
        [FromKeyedServices(Modules.VolunteerRequests)] IUnitOfWork unitOfWork,
        ILogger<ApproveVolunteerRequestHandler> logger)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
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

        await _unitOfWork.SaveChanges(cancellationToken);
            
        var requisitesDtos = volunteerRequestResult.Value.VolunteerInfo.Requisites
            .Select(r => new RequisiteDto(r.Name, r.Description));
            
        await _publishEndpoint.Publish(new ApproveVolunteerRequestEvent(
                volunteerRequestResult.Value.UserId,
                volunteerRequestResult.Value.VolunteerInfo.Experience.Value,
                requisitesDtos), 
            cancellationToken);
            
        _logger.LogInformation("Volunteer request {requestId} was approved successfully", 
            command.VolunteerRequestId);

        return Result.Success<ErrorList>();
    }
}
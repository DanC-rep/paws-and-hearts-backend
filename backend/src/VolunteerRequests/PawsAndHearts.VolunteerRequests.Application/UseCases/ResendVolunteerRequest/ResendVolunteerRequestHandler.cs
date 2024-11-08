using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.ResendVolunteerRequest;

public class ResendVolunteerRequestHandler : ICommandHandler<ResendVolunteerRequestCommand>
{
    private readonly IVolunteerRequestsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResendVolunteerRequestHandler> _logger;

    public ResendVolunteerRequestHandler(
        IVolunteerRequestsRepository repository,
        [FromKeyedServices(Modules.VolunteerRequests)] IUnitOfWork unitOfWork,
        ILogger<ResendVolunteerRequestHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        ResendVolunteerRequestCommand command, 
        CancellationToken cancellationToken = default)
    {
        var volunteerRequestResult = await _repository.GetById(command.VolunteerRequestId, cancellationToken);

        if (volunteerRequestResult.IsFailure)
            return volunteerRequestResult.Error.ToErrorList();

        if (volunteerRequestResult.Value.UserId != command.UserId)
            return Error.Failure("access.denied", "Request belong another user").ToErrorList();

        var result = volunteerRequestResult.Value.ResendVolunteerRequest();

        if (result.IsFailure)
            return result.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Volunteer request {requestId} was resented successfully",
            command.VolunteerRequestId);

        return Result.Success<ErrorList>();
    }
}
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.SendVolunteerRequestForRevision;

public class SendVolunteerRequestForRevisionHandler : ICommandHandler<SendVolunteerRequestForRevisionCommand>
{
    private readonly IVolunteerRequestsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SendVolunteerRequestForRevisionCommand> _validator;
    private readonly ILogger<SendVolunteerRequestForRevisionHandler> _logger;

    public SendVolunteerRequestForRevisionHandler(
        IVolunteerRequestsRepository repository,
        [FromKeyedServices(Modules.VolunteerRequests)] IUnitOfWork unitOfWork,
        IValidator<SendVolunteerRequestForRevisionCommand> validator,
        ILogger<SendVolunteerRequestForRevisionHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        SendVolunteerRequestForRevisionCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var volunteerRequestResult = await _repository
            .GetById(command.VolunteerRequestId, cancellationToken);

        if (volunteerRequestResult.IsFailure)
            return volunteerRequestResult.Error.ToErrorList();

        if (volunteerRequestResult.Value.AdminId != command.AdminId)
            return Error.Failure("access.denied", 
                "This request is under consideration by another admin").ToErrorList();

        var rejectionComment = RejectionComment.Create(command.RejectionComment).Value;

        var result = volunteerRequestResult.Value.SendForRevision(rejectionComment);

        if (result.IsFailure)
            return result.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("volunteer request {volunteerRequestId} was sent on revision by admin {adminId}",
            command.VolunteerRequestId, command.AdminId);

        return Result.Success<ErrorList>();
    }
}
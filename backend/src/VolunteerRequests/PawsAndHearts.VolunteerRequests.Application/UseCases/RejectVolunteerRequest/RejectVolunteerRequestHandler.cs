using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Domain.Entities;
using PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.RejectVolunteerRequest;

public class RejectVolunteerRequestHandler : ICommandHandler<RejectVolunteerRequestCommand>
{
    private readonly IUserRestrictionRepository _userRestrictionRepository;
    private readonly IVolunteerRequestsRepository _volunteerRequestsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RejectVolunteerRequestCommand> _validator;
    private readonly ILogger<RejectVolunteerRequestHandler> _logger;

    public RejectVolunteerRequestHandler(
        IUserRestrictionRepository userRestrictionRepository,
        IVolunteerRequestsRepository volunteerRequestsRepository,
        [FromKeyedServices(Modules.VolunteerRequests)] IUnitOfWork unitOfWork,
        IValidator<RejectVolunteerRequestCommand> validator,
        ILogger<RejectVolunteerRequestHandler> logger)
    {
        _userRestrictionRepository = userRestrictionRepository;
        _volunteerRequestsRepository = volunteerRequestsRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        RejectVolunteerRequestCommand command, 
        CancellationToken cancellationToken = default)
    {

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var volunteerRequestResult = await _volunteerRequestsRepository
                .GetById(command.VolunteerRequestId, cancellationToken);

            if (volunteerRequestResult.IsFailure)
                return volunteerRequestResult.Error.ToErrorList();

            if (volunteerRequestResult.Value.AdminId != command.UserId)
                return Error.Failure("access.denied",
                    "This request is under consideration by another admin").ToErrorList();

            var userRestrictionId = UserRestrictionId.NewId();

            var userRestrictionResult = UserRestriction
                .Create(userRestrictionId, volunteerRequestResult.Value.UserId);

            if (userRestrictionResult.IsFailure)
                return userRestrictionResult.Error.ToErrorList();

            await _userRestrictionRepository.Add(userRestrictionResult.Value, cancellationToken);

            var rejectionComment = RejectionComment.Create(command.RejectionComment).Value;

            var result = volunteerRequestResult.Value.Reject(rejectionComment);

            if (result.IsFailure)
                return result.Error.ToErrorList();

            await _unitOfWork.SaveChanges(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Volunteer request {requestId} was rejected", command.VolunteerRequestId);

            return Result.Success<ErrorList>();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            
            _logger.LogError("Can not reject volunteer request {requestId}", command.VolunteerRequestId);
            
            return Error.Failure("reject.volunteer.request", "Can not reject volunteer request").ToErrorList();
        }
    }
}
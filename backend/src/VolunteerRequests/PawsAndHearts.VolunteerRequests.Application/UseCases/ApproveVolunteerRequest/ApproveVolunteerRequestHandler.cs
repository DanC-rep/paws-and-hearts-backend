using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Accounts.Contracts;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.ApproveVolunteerRequest;

public class ApproveVolunteerRequestHandler : ICommandHandler<ApproveVolunteerRequestCommand>
{
    private readonly IVolunteerRequestsRepository _repository;
    private readonly IAccountsContract _accountsContract;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveVolunteerRequestHandler> _logger;

    public ApproveVolunteerRequestHandler(
        IVolunteerRequestsRepository repository,
        IAccountsContract accountsContract,
        [FromKeyedServices(Modules.VolunteerRequests)] IUnitOfWork unitOfWork,
        ILogger<ApproveVolunteerRequestHandler> logger)
    {
        _repository = repository;
        _accountsContract = accountsContract;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        ApproveVolunteerRequestCommand command, 
        CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var volunteerRequestResult = await _repository
                .GetById(command.VolunteerRequestId, cancellationToken);

            if (volunteerRequestResult.IsFailure)
                return volunteerRequestResult.Error.ToErrorList();

            var volunteerAccountResult = await _accountsContract
                .CreateVolunteerAccount(
                    volunteerRequestResult.Value.UserId,
                    volunteerRequestResult.Value.VolunteerInfo.Experience,
                    volunteerRequestResult.Value.VolunteerInfo.Requisites,
                    cancellationToken);

            if (volunteerAccountResult.IsFailure)
                return volunteerAccountResult.Error;

            var result = volunteerRequestResult.Value.Approve();

            if (result.IsFailure)
                return result.Error.ToErrorList();

            await _unitOfWork.SaveChanges(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Volunteer request {requestId} was approved successfully", 
                command.VolunteerRequestId);

            return Result.Success<ErrorList>();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            
            _logger.LogError("Can not approve volunteer request with id {requestId}", command.VolunteerRequestId);
            
            return Error.Failure("approve.volunteer.request", "Can not approve volunteer request").ToErrorList();
        }
    }
}
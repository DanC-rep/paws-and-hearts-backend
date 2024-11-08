using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Discussions.Contracts;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.TakeRequestForSubmit;

public class TakeRequestForSubmitHandler : ICommandHandler<TakeRequestForSubmitCommand>
{
    private readonly IDiscussionsContract _discussionsContract;
    private readonly IVolunteerRequestsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TakeRequestForSubmitHandler> _logger;

    public TakeRequestForSubmitHandler(
        IDiscussionsContract discussionsContract,
        IVolunteerRequestsRepository repository,
        [FromKeyedServices(Modules.VolunteerRequests)] IUnitOfWork unitOfWork,
        ILogger<TakeRequestForSubmitHandler> logger)
    {
        _discussionsContract = discussionsContract;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        TakeRequestForSubmitCommand command, 
        CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var volunteerRequestResult = await _repository.GetById(command.VolunteerRequestId, cancellationToken);

            if (volunteerRequestResult.IsFailure)
                return volunteerRequestResult.Error.ToErrorList();
            
            var discussionIdResult = await _discussionsContract.CreateDiscussion(
                    command.AdminId, 
                    volunteerRequestResult.Value.UserId, 
                    volunteerRequestResult.Value.Id,
                    cancellationToken);

            if (discussionIdResult.IsFailure)
                return discussionIdResult.Error;


            var result = volunteerRequestResult.Value
                .TakeRequestForSubmit(command.AdminId, discussionIdResult.Value);

            if (result.IsFailure)
                return result.Error.ToErrorList();

            await _unitOfWork.SaveChanges(cancellationToken);
            
            transaction.Commit();
            
            _logger.LogInformation("Volunteer request {requestId} was taken for submit by admin {adminId}",
                command.VolunteerRequestId, command.AdminId);

            return Result.Success<ErrorList>();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            
            _logger.LogError("Can not take request for submit with id {requestId}", command.VolunteerRequestId);

            return Error.Failure("take.request.for.submit", "Can not take request for submit").ToErrorList();
        }
    }
}
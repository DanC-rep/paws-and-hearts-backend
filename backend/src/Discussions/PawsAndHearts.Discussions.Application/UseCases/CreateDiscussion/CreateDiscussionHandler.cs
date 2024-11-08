using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Accounts.Contracts;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.Discussions.Domain.Entities;
using PawsAndHearts.Discussions.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.Discussions.Application.UseCases.CreateDiscussion;

public class CreateDiscussionHandler : ICommandHandler<Guid, CreateDiscussionCommand>
{
    private readonly IAccountsContract _accountsContract;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDiscussionsRepository _repository;
    private readonly ILogger<CreateDiscussionHandler> _logger;

    public CreateDiscussionHandler(
        IAccountsContract accountsContract,
        IDiscussionsRepository repository,
        [FromKeyedServices(Modules.Discussions)] IUnitOfWork unitOfWork,
        ILogger<CreateDiscussionHandler> logger)
    {
        _accountsContract = accountsContract;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateDiscussionCommand command, 
        CancellationToken cancellationToken = default)
    {
        var isDiscussionExists = await _repository.GetByRelationId(command.RelationId, cancellationToken);

        if (isDiscussionExists.IsSuccess)
            return isDiscussionExists.Error.ToErrorList();

        var firstMember = await _accountsContract.GetUserById(command.FirstMember, cancellationToken);
        var secondMember = await _accountsContract.GetUserById(command.SecondMember, cancellationToken);

        if (firstMember.IsFailure)
            return firstMember.Error;

        if (secondMember.IsFailure)
            return secondMember.Error;

        var discussionId = DiscussionId.NewId();

        var users = new Users(command.FirstMember, command.SecondMember);

        var discussion = new Discussion(discussionId, users, command.RelationId);
        
        var result = await _repository.Add(discussion, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Discussion was created successfully with id {discussionId}", discussionId);

        return result;
    }
}
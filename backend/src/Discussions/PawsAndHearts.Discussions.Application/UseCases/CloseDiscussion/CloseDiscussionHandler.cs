using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Discussions.Application.UseCases.CloseDiscussion;

public class CloseDiscussionHandler : ICommandHandler<CloseDiscussionCommand>
{
    private readonly IDiscussionsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CloseDiscussionHandler> _logger;

    public CloseDiscussionHandler(
        IDiscussionsRepository repository,
        [FromKeyedServices(Modules.Discussions)] IUnitOfWork unitOfWork,
        ILogger<CloseDiscussionHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        CloseDiscussionCommand command, 
        CancellationToken cancellationToken = default)
    {
        var discussionResult = await _repository.GetById(command.DiscussionId, cancellationToken);

        if (discussionResult.IsFailure)
            return discussionResult.Error.ToErrorList();

        var result = discussionResult.Value.CloseDiscussion();

        if (result.IsFailure)
            return result.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Discussion {discussionId} was closed successfully", command.DiscussionId);

        return Result.Success<ErrorList>();
    }
}
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Discussions.Application.UseCases.DeleteMessage;

public class DeleteMessageHandler : ICommandHandler<DeleteMessageCommand>
{
    private readonly IDiscussionsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMessageHandler> _logger;

    public DeleteMessageHandler(
        IDiscussionsRepository repository,
        [FromKeyedServices(Modules.Discussions)] IUnitOfWork unitOfWork,
        ILogger<DeleteMessageHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        DeleteMessageCommand command, 
        CancellationToken cancellationToken = default)
    {
        var discussionResult = await _repository.GetById(command.DiscussionId, cancellationToken);

        if (discussionResult.IsFailure)
            return discussionResult.Error.ToErrorList();

        var result = discussionResult.Value.DeleteComment(command.UserId, command.MessageId);
        
        if (result.IsFailure)
            return result.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Message {messageId} was deleted in discussion {discussionId}",
            command.MessageId, command.DiscussionId);

        return Result.Success<ErrorList>();
    }
}
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.Discussions.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Discussions.Application.UseCases.UpdateMessage;

public class UpdateMessageHandler : ICommandHandler<UpdateMessageCommand>
{
    private readonly IDiscussionsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateMessageCommand> _validator;
    private readonly ILogger<UpdateMessageHandler> _logger;

    public UpdateMessageHandler(
        IDiscussionsRepository repository,
        [FromKeyedServices(Modules.Discussions)] IUnitOfWork unitOfWork,
        IValidator<UpdateMessageCommand> validator,
        ILogger<UpdateMessageHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        UpdateMessageCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var discussionResult = await _repository.GetById(command.DiscussionId, cancellationToken);

        if (discussionResult.IsFailure)
            return discussionResult.Error.ToErrorList();

        var messageText = MessageText.Create(command.Message).Value;

        var result = discussionResult.Value.EditComment(command.UserId, command.MessageId, messageText);

        if (result.IsFailure)
            return result.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Message {messageId} was edited in discussion {discussionid}",
            command.MessageId, command.DiscussionId);

        return Result.Success<ErrorList>();
    }
}
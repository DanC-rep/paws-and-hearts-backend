using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.Discussions.Domain.Entities;
using PawsAndHearts.Discussions.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.Discussions.Application.UseCases.SendMessage;

public class SendMessageHandler : ICommandHandler<Guid, SendMessageCommand>
{
    private readonly IDiscussionsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SendMessageCommand> _validator;
    private readonly ILogger<SendMessageHandler> _logger;

    public SendMessageHandler(
        IDiscussionsRepository repository,
        [FromKeyedServices(Modules.Discussions)] IUnitOfWork unitOfWork,
        IValidator<SendMessageCommand> validator,
        ILogger<SendMessageHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        SendMessageCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var discussionResult = await _repository.GetById(command.DiscussionId, cancellationToken);

        if (discussionResult.IsFailure)
            return discussionResult.Error.ToErrorList();

        var messageId = MessageId.NewId();

        var messageText = MessageText.Create(command.Message).Value;

        var message = new Message(messageId, command.UserId, messageText, DateTime.UtcNow);

        var result = discussionResult.Value.SendComment(message);

        if (discussionResult.IsFailure)
            return result.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("The message {messageId} was sent successfully", messageId);

        return messageId.Value;
    }
}
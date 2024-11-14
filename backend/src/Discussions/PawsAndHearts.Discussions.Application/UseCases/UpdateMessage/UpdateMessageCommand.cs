using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Discussions.Application.UseCases.UpdateMessage;

public record UpdateMessageCommand(Guid DiscussionId, Guid MessageId, Guid UserId, string Message) : ICommand;
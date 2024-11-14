using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Discussions.Application.UseCases.DeleteMessage;

public record DeleteMessageCommand(Guid DiscussionId, Guid MessageId, Guid UserId) : ICommand;
using ICommand = PawsAndHearts.Core.Abstractions.ICommand;

namespace PawsAndHearts.Discussions.Application.UseCases.SendMessage;

public record SendMessageCommand(Guid DiscussionId, Guid UserId, string Message) : ICommand;
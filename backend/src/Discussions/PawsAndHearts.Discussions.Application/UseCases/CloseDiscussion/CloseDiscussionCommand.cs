using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Discussions.Application.UseCases.CloseDiscussion;

public record CloseDiscussionCommand(Guid DiscussionId) : ICommand;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Discussions.Application.UseCases.CreateDiscussion;

public record CreateDiscussionCommand(Guid FirstMember, Guid SecondMember, Guid RelationId) : ICommand;
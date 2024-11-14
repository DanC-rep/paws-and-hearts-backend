using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Discussions.Application.Queries.GetDiscussionByRelationId;

public record GetDiscussionByRelationIdQuery(Guid RelationId) : IQuery;
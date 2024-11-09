using CSharpFunctionalExtensions;
using PawsAndHearts.Discussions.Domain.Entities;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.Discussions.Application.Interfaces;

public interface IDiscussionsRepository
{
    Task<Guid> Add(Discussion discussion, CancellationToken cancellationToken = default);
    
    Task<Result<Discussion, Error>> GetById(DiscussionId discussionId, CancellationToken cancellationToken = default);

    Task<Result<Discussion, Error>> GetByRelationId(Guid relationId, CancellationToken cancellationToken = default);
    
    Result<Guid, Error> Delete(Discussion discussion, CancellationToken cancellationToken = default);
}
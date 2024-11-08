using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Discussions.Contracts;

public interface IDiscussionsContract
{
    Task<Result<Guid, ErrorList>> CreateDiscussion(
        Guid firstMember, 
        Guid secondMember, 
        Guid relationId,
        CancellationToken cancellationToken = default);
    
    
}
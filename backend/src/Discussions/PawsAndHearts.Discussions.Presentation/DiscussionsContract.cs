using CSharpFunctionalExtensions;
using PawsAndHearts.Discussions.Application.UseCases.CreateDiscussion;
using PawsAndHearts.Discussions.Contracts;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Discussions.Presentation;

public class DiscussionsContract : IDiscussionsContract
{
    private readonly CreateDiscussionHandler _createDiscussionHandler;

    public DiscussionsContract(CreateDiscussionHandler createDiscussionHandler)
    {
        _createDiscussionHandler = createDiscussionHandler;
    }
    
    public async Task<Result<Guid, ErrorList>> CreateDiscussion(
        Guid firstMember, 
        Guid secondMember, 
        Guid relationId,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateDiscussionCommand(firstMember, secondMember, relationId);
        
        return await _createDiscussionHandler.Handle(command, cancellationToken);
    }
}
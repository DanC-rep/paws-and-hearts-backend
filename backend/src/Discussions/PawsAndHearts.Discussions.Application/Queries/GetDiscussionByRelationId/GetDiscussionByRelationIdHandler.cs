using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.Discussions.Contracts.Dtos;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Discussions.Application.Queries.GetDiscussionByRelationId;

public class GetDiscussionByRelationIdHandler 
    : IQueryHandlerWithResult<DiscussionDto, GetDiscussionByRelationIdQuery>
{
    private readonly IDiscussionsReadDbContext _readDbContext;
    
    public GetDiscussionByRelationIdHandler(IDiscussionsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }
    
    public async Task<Result<DiscussionDto, ErrorList>> Handle(
        GetDiscussionByRelationIdQuery query, 
        CancellationToken cancellationToken = default)
    {
        var discussionDto = await _readDbContext.Discussions
            .Include(d => d.Messages)
            .FirstOrDefaultAsync(d => d.RelationId == query.RelationId, cancellationToken);

        if (discussionDto is null)
            return Errors.General.NotFound(null, "discussion").ToErrorList();

        return discussionDto;
    }
}